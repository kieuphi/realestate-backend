using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace General.Application.PropertyFavorite.Queries
{
    public class GetPagingPropertyFavoriteByUserIdQuery : IRequest<PaginatedList<ListPropertyFavoriteModel>>
    {
        public PagingPropertyFavoriteModel Model { set; get; }
    }

    public class GetPagingPropertyFavoriteByUserIdQueryHandler : IRequestHandler<GetPagingPropertyFavoriteByUserIdQuery, PaginatedList<ListPropertyFavoriteModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyFavoriteEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly ICurrentUserService _userService;

        public GetPagingPropertyFavoriteByUserIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyFavoriteEntity> repository,
            ICommonFunctionService commonFunctionService,
            ICurrentUserService userService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PaginatedList<ListPropertyFavoriteModel>> Handle(GetPagingPropertyFavoriteByUserIdQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            var propertyFavorite = await _context.PropertyFavorite
                        .Where(x => x.IsDeleted == DeletedStatus.False && x.UserId == Guid.Parse(_userService.UserId))
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ListPropertyFavoriteModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            List<ListPropertyFavoriteModel> result = new List<ListPropertyFavoriteModel>();

            if (propertyFavorite.Count() > 0)
            {
                var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
                var districtsList = JObject.Parse(File.ReadAllText(mapJSON))["districts"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
                var wardsList = JObject.Parse(File.ReadAllText(mapJSON))["wards"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();

                var properties = _context.Property.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active).ToList();

                for (int i = 0; i < propertyFavorite.Count(); i++)
                {
                    for(int j = 0; j < properties.Count(); j++)
                    {
                        if(propertyFavorite[i].PropertyId == properties[j].Id)
                        {
                            var timeForPost = _context.TimeForPost.Where(x => x.Id == properties[j].TimeForPostId).FirstOrDefault();

                            propertyFavorite[i].TimeForPostId = properties[j].TimeForPostId;
                            propertyFavorite[i].TimeForPostValue = timeForPost.Value;
                            propertyFavorite[i].TimeForPostName = timeForPost.DisplayName;

                            // calcu time for post
                            DateTime postingStartDate = DateTime.Parse(properties[j].ApproveDate.ToString());
                            DateTime postingExpiryDate = postingStartDate.AddDays(Convert.ToDouble(timeForPost.Value));

                            propertyFavorite[i].TimeRemain = Math.Round(Convert.ToDecimal((postingExpiryDate - DateTime.Now).TotalDays), 0);

                            // =================
                            // ADMINISTRATIVE
                            // =================
                            // ward
                            if (properties[j].WardCode != null)
                            {
                                if (wardsList.Where(x => x.code.ToString() == properties[j].WardCode).FirstOrDefault() != null)
                                {
                                    propertyFavorite[i].WardCode = properties[j].WardCode;
                                    propertyFavorite[i].WardName = wardsList.Where(x => x.code.ToString() == properties[j].WardCode).FirstOrDefault().nameWithType.ToString();
                                    propertyFavorite[i].Location = propertyFavorite[i].WardName;
                                }
                            }

                            // district
                            if (properties[j].DistrictCode != null)
                            {
                                if (districtsList.Where(x => x.code.ToString() == properties[j].DistrictCode).FirstOrDefault() != null)
                                {
                                    propertyFavorite[i].DistrictCode = properties[j].DistrictCode;
                                    propertyFavorite[i].DistrictName = districtsList.Where(x => x.code.ToString() == properties[j].DistrictCode).FirstOrDefault().nameWithType.ToString();
                                    propertyFavorite[i].Location = propertyFavorite[i].Location + ", " + propertyFavorite[i].DistrictName;
                                }
                            }

                            // province
                            if (properties[j].ProvinceCode != null)
                            {
                                if (provincesList.Where(x => x.code.ToString() == properties[j].ProvinceCode).FirstOrDefault() != null)
                                {
                                    propertyFavorite[i].ProvinceCode = properties[j].ProvinceCode;
                                    propertyFavorite[i].ProvinceName = provincesList.Where(x => x.code.ToString() == properties[j].ProvinceCode).FirstOrDefault().nameWithType.ToString();
                                    propertyFavorite[i].Location = propertyFavorite[i].Location + ", " + propertyFavorite[i].ProvinceName;
                                }
                            }

                            // =================
                            // MASTER DATA
                            // =================
                            // bedrooms
                            if (properties[j].BedroomId != null)
                            {
                                var bedroomTypes = JObject.Parse(File.ReadAllText(masterData))["bedroomTypes"].Where(n => n["id"].Value<string>() == properties[j].BedroomId);
                                if (bedroomTypes.ToList().Count > 0)
                                {
                                    propertyFavorite[i].BedroomId = properties[j].BedroomId;
                                    propertyFavorite[i].BedroomVi = bedroomTypes.Select(n => new { Value = n["bedroomTypesVi"] }).FirstOrDefault().Value.ToString();
                                    propertyFavorite[i].BedroomEn = bedroomTypes.Select(n => new { Value = n["bedroomTypesEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // bathrooms
                            if (properties[j].BedroomId != null)
                            {
                                var bathroomTypes = JObject.Parse(File.ReadAllText(masterData))["bathroomTypes"].Where(n => n["id"].Value<string>() == properties[j].BathroomId);
                                if (bathroomTypes.ToList().Count > 0)
                                {
                                    propertyFavorite[i].BedroomId = properties[j].BedroomId;
                                    propertyFavorite[i].BathroomVi = bathroomTypes.Select(n => new { Value = n["bathroomTypesVi"] }).FirstOrDefault().Value.ToString();
                                    propertyFavorite[i].BathroomEn = bathroomTypes.Select(n => new { Value = n["bathroomTypesEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // property type
                            if (properties[j].PropertyTypeId != null)
                            {
                                var propertyServiceType = JObject.Parse(File.ReadAllText(masterData))["propertyType"].Where(n => n["id"].Value<string>() == properties[j].PropertyTypeId);
                                if (propertyServiceType.ToList().Count > 0)
                                {
                                    propertyFavorite[i].PropertyTypeId = properties[j].PropertyTypeId;
                                    propertyFavorite[i].PropertyTypeVi = propertyServiceType.Select(n => new { Value = n["propertyTypeVi"] }).FirstOrDefault().Value.ToString();
                                    propertyFavorite[i].PropertyTypeEn = propertyServiceType.Select(n => new { Value = n["propertyTypeEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // transaction type
                            if (properties[j].TransactionTypeId != null)
                            {
                                var transactionType = JObject.Parse(File.ReadAllText(masterData))["transactionType"].Where(n => n["id"].Value<string>() == properties[j].TransactionTypeId);
                                if (transactionType.ToList().Count > 0)
                                {
                                    propertyFavorite[i].TransactionTypeId = properties[j].TransactionTypeId;
                                    propertyFavorite[i].TransactionTypeVi = transactionType.Select(n => new { Value = n["transactionTypeVi"] }).FirstOrDefault().Value.ToString();
                                    propertyFavorite[i].TransactionTypeEn = transactionType.Select(n => new { Value = n["transactionTypeEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // currency
                            if (properties[j].CurrencyId != null)
                            {
                                var currency = JObject.Parse(File.ReadAllText(masterData))["currency"].Where(n => n["id"].Value<string>() == properties[j].CurrencyId);
                                if (currency.ToList().Count > 0)
                                {
                                    propertyFavorite[i].CurrencyId = properties[j].CurrencyId;
                                    propertyFavorite[i].CurrencyName = currency.Select(n => new { Value = n["currencyName"] }).FirstOrDefault().Value.ToString();
                                    propertyFavorite[i].CurrencyNotation = currency.Select(n => new { Value = n["notation"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            propertyFavorite[i].Title = properties[j].Title;
                            propertyFavorite[i].PropertyNumber = properties[j].PropertyNumber;
                            propertyFavorite[i].CoverImage = properties[j].CoverImage;
                            propertyFavorite[i].PropertyAddressVi = properties[j].PropertyAddressVi;
                            propertyFavorite[i].PropertyAddressEn = properties[j].PropertyAddressEn;
                            propertyFavorite[i].Price = properties[j].Price;
                            propertyFavorite[i].Longitude = properties[j].Longitude;
                            propertyFavorite[i].Latitude = properties[j].Latitude;

                            result.Add(propertyFavorite[i]);

                            break;
                        }
                    }
                }

            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<ListPropertyFavoriteModel>(result, result.Count, 1, result.Count);
            }

            var paginatedList = PaginatedList<ListPropertyFavoriteModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);
            return paginatedList;
        }
    }
}
