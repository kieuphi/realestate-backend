using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using General.Domain.Models.PropertyElementModels;
using General.Domain.Enums;
using General.Application.Common.Results;
using System.Collections.Generic;

namespace General.Application.Property.Queries
{
    public class GetPropertyByIdQuery : IRequest<PropertyModel>
    {
        public Guid Id { set; get; }
        public bool IsAdmin { get; set; }
    }

    public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _userService;
        private readonly IConvertVietNameseService _convertVietnameseService;

        public GetPropertyByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IIdentityService identityService,
            ICurrentUserService userService,
            IConvertVietNameseService convertVietnameseService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _convertVietnameseService = convertVietnameseService ?? throw new ArgumentNullException(nameof(convertVietnameseService));
        }

        public async Task<PropertyModel> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var result = await _context.Property
                    .Where(x => x.Id == request.Id)
                    .AsNoTracking()
                    .ProjectTo<PropertyModel>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();
            if (request.IsAdmin == false)
            {
                if (result.IsApprove != PropertyApproveStatus.Active && (string.IsNullOrEmpty(_userService.UserName) || _userService.UserName != result.CreateBy))
                {
                    return null;
                }
            }

            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            if (result != null)
            {
                result.CoverImageUrl = !string.IsNullOrEmpty(result.CoverImage) ? host + result.CoverImage : "";
                result.LotSizeFeet = Decimal.Round((result.LotSize * (decimal)10.76391) ?? 0, 0);

                if (result.IsApprove == PropertyApproveStatus.Active)
                {
                    result.ApproveStatusName = "Active";
                }
                else
                {
                    result.ApproveStatusName = "Inactive";
                }
                var propertyViewCount = await _context.PropertyViewCount.Where(x => x.PropertyId == result.Id).FirstOrDefaultAsync();
                result.ViewCount = 0;
                if (propertyViewCount != null)
                {
                    result.ViewCount = propertyViewCount.ViewCount;
                }

                if ((result.TimeForPostId != null || result.TimeForPostId != Guid.Empty) && result.IsApprove == PropertyApproveStatus.Active)
                {
                    var timeForPost = _context.TimeForPost.Where(x => x.Id == result.TimeForPostId).FirstOrDefault();

                    if (timeForPost != null)
                    {
                        result.TimeForPostValue = timeForPost.Value;
                        result.TimeForPostName = timeForPost.DisplayName;

                        if (result.ApproveDate != null)
                        {
                            // calcu time for post
                            DateTime postingStartDate = DateTime.Parse(result.ApproveDate.ToString());
                            DateTime postingExpiryDate = postingStartDate.AddDays(Convert.ToDouble(timeForPost.Value));

                            result.TimeRemain = Math.Round(Convert.ToDecimal((postingExpiryDate - DateTime.Now).TotalDays), 0);
                        }
                    }
                }

                // =================
                // DB DATA
                // =================
                // project
                if (result.ProjectId != null || result.ProjectId != Guid.Empty)
                {
                    var project = _context.Project.Where(x => x.Id == result.ProjectId).FirstOrDefault();
                    if (project != null)
                    {
                        result.ProjectVi = project.ProjectVi;
                        result.ProjectEn = project.ProjectEn;
                    }
                }

                // property View
                var propertyView = _context.PropertyView.Where(x => x.PropertyId == result.Id).ProjectTo<PropertyViewModel>(_mapper.ConfigurationProvider).ToList();
                if (propertyView != null && propertyView.Count() > 0)
                {
                    result.PropertyViews = propertyView;

                    var view = JObject.Parse(File.ReadAllText(masterData))["view"].ToArray();
                    if (view != null)
                    {
                        for (int i = 0; i < result.PropertyViews.Count(); i++)
                        {
                            for (int j = 0; j < view.Count(); j++)
                            {
                                if (result.PropertyViews[i].ViewId == view[j].Value<string>("id").ToString())
                                {
                                    result.PropertyViews[i].viewVi = view[j].Value<string>("viewVi").ToString();
                                    result.PropertyViews[i].viewEn = view[j].Value<string>("viewEn").ToString();
                                    
                                    break;
                                }
                            }
                        }
                    }
                }

                // amenities nearby
                var propertyAmenitiesNearby = _context.PropertyAmenitiesNearby.Where(x => x.PropertyId == result.Id).ProjectTo<PropertyAmenitiesNearbyModel>(_mapper.ConfigurationProvider).ToList();
                if (propertyAmenitiesNearby != null && propertyAmenitiesNearby.Count() > 0)
                {
                    result.PropertyAmenitiesNearbys = propertyAmenitiesNearby;

                    var amenitiesNearby = JObject.Parse(File.ReadAllText(masterData))["amenitiesNearby"].ToArray();
                    if (amenitiesNearby != null)
                    {
                        for (int i = 0; i < result.PropertyAmenitiesNearbys.Count(); i++)
                        {
                            for (int j = 0; j < amenitiesNearby.Count(); j++)
                            {
                                if (result.PropertyAmenitiesNearbys[i].AmenitiesNearbyId == amenitiesNearby[j].Value<string>("id").ToString())
                                {
                                    result.PropertyAmenitiesNearbys[i].AmenitiesNearbyVi = amenitiesNearby[j].Value<string>("amenitiesNearbyVi").ToString();
                                    result.PropertyAmenitiesNearbys[i].AmenitiesNearbyEn = amenitiesNearby[j].Value<string>("amenitiesNearbyEn").ToString();

                                    break;
                                }
                            }
                        }
                    }
                }

                // property image
                var propertyImage = _context.PropertyImage.Where(x => x.PropertyId == result.Id).ProjectTo<PropertyImageModel>(_mapper.ConfigurationProvider).ToList();
                if (propertyImage != null)
                {
                    result.PropertyImages = propertyImage;

                    for (int i = 0; i < result.PropertyImages.Count(); i++)
                    {
                        result.PropertyImages[i].ImagesUrl = !string.IsNullOrEmpty(result.PropertyImages[i].ImagesPath) ? host + result.PropertyImages[i].ImagesPath : "";
                    }
                }

                // seller
                var propertySellers = _context.PropertySeller.Where(x => x.PropertyId == result.Id).ProjectTo<PropertySellerModel>(_mapper.ConfigurationProvider).ToList();
                if ((propertySellers != null && propertySellers.Count() > 0) || result.SupplierId != null)
                {
                    result.PropertySellers = new List<PropertySellerModel>();

                    List<Guid> listSellerId = propertySellers.Select(x => x.UserId).ToList();
                    if (request.IsAdmin == false)
                    {
                        if (result.IsShowSupplier != null && result.IsShowSupplier == true &&
                            result.SupplierId != null && result.SupplierId != Guid.Empty)
                        {
                            listSellerId.Add(result.SupplierId);
                        }
                    }

                    var listSeller = await _context.ProfileInformation.Where(x => listSellerId.Contains(x.UserId)).ToListAsync();

                    var socialNetworkUsersEntity = await _context.SocialNetworkUser
                        .ProjectTo<SocialNetworkUserModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                    for (int i = 0; i < listSeller.Count(); i++)
                    {
                        PropertySellerModel item = new PropertySellerModel();
                        var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(listSeller[i].UserId.ToString()));
                        var socialNetworkUsers = socialNetworkUsersEntity
                                        .Where(x => x.ProfileId == listSeller[i].Id)
                                        .ToList();

                        if (socialNetworkUsers.Count() > 0)
                        {
                            var socialNetworks = await _context.SocialNetwork.ToListAsync();

                            for (int x = 0; x < socialNetworkUsers.Count(); x++)
                            {
                                string icon = socialNetworks.Where(b => b.Id == socialNetworkUsers[x].SocialNetworkId).FirstOrDefault().ICon;
                                socialNetworkUsers[x].SocialNetworkIconUrl = !string.IsNullOrEmpty(icon) ? host + icon : "";
                            }
                        }

                        result.PropertySellers.Add(new PropertySellerModel
                        {
                            Id = listSeller[i].Id,
                            PropertyId = result.Id,
                            UserId = listSeller[i].UserId,
                            Avatar = listSeller[i].Avatar,
                            AvatarUrl = !string.IsNullOrEmpty(listSeller[i].Avatar) ? host + listSeller[i].Avatar : "",
                            Email = user != null ? user.Email : "",
                            FirstName = listSeller[i].FirstName,
                            LastName = listSeller[i].LastName,
                            PhoneNumber1 = listSeller[i].PhoneNumber1,
                            PhoneNumber2 = listSeller[i].PhoneNumber2,
                            PhoneNumber3 = listSeller[i].PhoneNumber3,
                            TitleVi = listSeller[i].TitleVi,
                            TitleEn = listSeller[i].TitleEn,
                            TitleDescriptionVi = listSeller[i].TitleDescriptionVi,
                            TitleDescriptionEn = listSeller[i].TitleDescriptionEn,
                            SocialNetworkUsers = socialNetworkUsers,
                            Agency = listSeller[i].Agency
                        });
                    }
                }

                // supplier
                if (result.SupplierId != null || result.SupplierId != Guid.Empty)
                {
                    var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(result.SupplierId.ToString()));
                    if (user != null)
                    {
                        var supplier = _context.ProfileInformation.Where(x => x.UserId.ToString() == user.Id).FirstOrDefault();

                        if (supplier != null)
                        {
                            result.SupplierAvatar = supplier.Avatar;
                            result.SupplierFirstName = supplier.FirstName;
                            result.SupplierLastName = supplier.LastName;
                            result.SuppierPhoneNumber1 = supplier.PhoneNumber1;
                            result.SuppierPhoneNumber2 = supplier.PhoneNumber2;
                            result.SuppierTitleVi = supplier.TitleVi;
                            result.SuppierTitleEn = supplier.TitleEn;
                            result.SuppierTitleDescriptionVi = supplier.TitleDescriptionVi;
                            result.SuppierTitleDescriptionEn = supplier.TitleDescriptionEn;
                            result.SupplierEmail = user.Email;
                        }
                    }
                }

                // =================
                // ADMINISTRATIVE
                // =================
                // province
                if (result.ProvinceCode != null)
                {
                    var province = JObject.Parse(File.ReadAllText(mapJSON))["cities"]
                        .Where(n => n["code"].Value<string>() == result.ProvinceCode).Select(n => new { nameWithType = n["nameWithType"], name = n["name"] }).FirstOrDefault();
                    if (province != null)
                    {
                        result.ProvinceName = province.name.ToString();
                        result.ProvinceNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(province.nameWithType.ToString(), true);
                    }
                }

                // district
                if (result.DistrictCode != null)
                {
                    var district = JObject.Parse(File.ReadAllText(mapJSON))["districts"]
                        .Where(n => n["code"].Value<string>() == result.DistrictCode).Select(n => new { nameWithType = n["nameWithType"] }).FirstOrDefault();
                    if (district != null)
                    {
                        result.DistrictName = district.nameWithType.ToString();
                        result.DistrictNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(district.nameWithType.ToString());
                    }
                }

                // ward
                if (result.WardCode != null)
                {
                    var ward = JObject.Parse(File.ReadAllText(mapJSON))["wards"]
                        .Where(n => n["code"].Value<string>() == result.WardCode).Select(n => new { nameWithType = n["nameWithType"] }).FirstOrDefault();
                    if (ward != null)
                    {
                        result.WardName = ward.nameWithType.ToString();
                        result.WardNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(ward.nameWithType.ToString());
                    }
                }

                // =================
                // MASTER DATA
                // =================
                // transaction type
                if (result.TransactionTypeId != null)
                {
                    var transactionType = JObject.Parse(File.ReadAllText(masterData))["transactionType"].Where(n => n["id"].Value<string>() == result.TransactionTypeId);
                    if (transactionType.ToList().Count > 0)
                    {
                        result.TransactionTypeVi = transactionType.Select(n => new { Value = n["transactionTypeVi"] }).FirstOrDefault().Value.ToString();

                        result.TransactionTypeEn = transactionType.Select(n => new { Value = n["transactionTypeEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // property type
                if (result.PropertyTypeId != null)
                {
                    var propertyServiceType = JObject.Parse(File.ReadAllText(masterData))["propertyType"].Where(n => n["id"].Value<string>() == result.PropertyTypeId);
                    if (propertyServiceType.ToList().Count > 0)
                    {
                        result.PropertyTypeVi = propertyServiceType.Select(n => new { Value = n["propertyTypeVi"] }).FirstOrDefault().Value.ToString();

                        result.PropertyTypeEn = propertyServiceType.Select(n => new { Value = n["propertyTypeEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // currency
                if (result.CurrencyId != null)
                {
                    var currency = JObject.Parse(File.ReadAllText(masterData))["currency"].Where(n => n["id"].Value<string>() == result.CurrencyId);
                    if (currency.ToList().Count > 0)
                    {
                        result.CurrencyName = currency.Select(n => new { Value = n["currencyName"] }).FirstOrDefault().Value.ToString();

                        result.CurrencyNotation = currency.Select(n => new { Value = n["notation"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // bedrooms
                if (result.BedroomId != null)
                {
                    var bedroomTypes = JObject.Parse(File.ReadAllText(masterData))["bedroomTypes"].Where(n => n["id"].Value<string>() == result.BedroomId);
                    if (bedroomTypes.ToList().Count > 0)
                    {
                        result.BedroomVi = bedroomTypes.Select(n => new { Value = n["bedroomTypesVi"] }).FirstOrDefault().Value.ToString();

                        result.BedroomEn = bedroomTypes.Select(n => new { Value = n["bedroomTypesEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // bathrooms
                if (result.BathroomId != null)
                {
                    var bathroomTypes = JObject.Parse(File.ReadAllText(masterData))["bathroomTypes"].Where(n => n["id"].Value<string>() == result.BathroomId);
                    if (bathroomTypes.ToList().Count > 0)
                    {
                        result.BathroomVi = bathroomTypes.Select(n => new { Value = n["bathroomTypesVi"] }).FirstOrDefault().Value.ToString();

                        result.BathroomEn = bathroomTypes.Select(n => new { Value = n["bathroomTypesEn"] }).FirstOrDefault().Value.ToString();
                    }
                }
            }

            return result;
        }
    }
}
