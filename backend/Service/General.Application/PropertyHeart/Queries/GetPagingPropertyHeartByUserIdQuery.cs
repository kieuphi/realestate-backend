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

namespace General.Application.PropertyHeart.Queries
{
    public class GetPagingPropertyHeartByUserIdQuery : IRequest<PaginatedList<ListPropertyHeartModel>>
    {
        public PagingPropertyHeartModel Model { set; get; }
    }

    public class GetPagingPropertyHeartByUserIdQueryHandler : IRequestHandler<GetPagingPropertyHeartByUserIdQuery, PaginatedList<ListPropertyHeartModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyHeartEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly ICurrentUserService _userService;
        private readonly IConvertVietNameseService _convertVietnameseService;

        public GetPagingPropertyHeartByUserIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyHeartEntity> repository,
            ICommonFunctionService commonFunctionService,
            ICurrentUserService userService,
            IConvertVietNameseService convertVietnameseService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _convertVietnameseService = convertVietnameseService ?? throw new ArgumentNullException(nameof(convertVietnameseService));
        }

        public async Task<PaginatedList<ListPropertyHeartModel>> Handle(GetPagingPropertyHeartByUserIdQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var masterData = "MasterData.json";
            var mapJSON = "map.json";
            string host = _commonFunctionService.ConvertImageUrl("");

            var propertyHeart = await _context.PropertyHeart
                        .Where(x => x.IsDeleted == DeletedStatus.False && x.UserId == Guid.Parse(_userService.UserId))
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ListPropertyHeartModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();
            List<Guid> listPropertyId = propertyHeart.Select(x => x.PropertyId).ToList();

            List<ListPropertyHeartModel> result = new List<ListPropertyHeartModel>();
            if (propertyHeart.Count == 0)
            {
                return new PaginatedList<ListPropertyHeartModel>(result, result.Count, 1, result.Count);
            }

            var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
            var districtsList = JObject.Parse(File.ReadAllText(mapJSON))["districts"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
            var wardsList = JObject.Parse(File.ReadAllText(mapJSON))["wards"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();

            var listProperty = await _context.Property
                .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active
                    && listPropertyId.Contains(x.Id))
                .ToListAsync();            

            for (int i = 0; i < propertyHeart.Count; i++)
            {
                PropertyEntity propertyInfo = listProperty.Where(x => x.Id == propertyHeart[i].PropertyId).FirstOrDefault();
                if (propertyInfo == null)
                {
                    continue;
                }

                // =================
                // ADMINISTRATIVE
                // =================
                // ward
                if (propertyInfo.WardCode != null)
                {
                    var ward = wardsList.Where(x => x.code.ToString() == propertyInfo.WardCode).FirstOrDefault();
                    if (ward != null)
                    {
                        propertyHeart[i].WardCode = propertyInfo.WardCode;
                        propertyHeart[i].WardName = ward.nameWithType.ToString();
                        propertyHeart[i].WardNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(ward.nameWithType.ToString());
                        propertyHeart[i].Location = propertyHeart[i].WardName;
                        propertyHeart[i].LocationEn = propertyHeart[i].WardNameEn;
                    }
                }

                // district
                if (propertyInfo.DistrictCode != null)
                {
                    var district = districtsList.Where(x => x.code.ToString() == propertyInfo.DistrictCode).FirstOrDefault();
                    if (district != null)
                    {
                        propertyHeart[i].DistrictCode = propertyInfo.DistrictCode;
                        propertyHeart[i].DistrictName = district.nameWithType.ToString();
                        propertyHeart[i].DistrictNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(district.nameWithType.ToString());
                        propertyHeart[i].Location = propertyHeart[i].Location + ", " + propertyHeart[i].DistrictName;
                        propertyHeart[i].LocationEn = propertyHeart[i].LocationEn + ", " + propertyHeart[i].DistrictNameEn;
                    }
                }

                // province
                if (propertyInfo.ProvinceCode != null)
                {
                    var province = provincesList.Where(x => x.code.ToString() == propertyInfo.ProvinceCode).FirstOrDefault();
                    if (province != null)
                    {
                        propertyHeart[i].ProvinceCode = propertyInfo.ProvinceCode;
                        propertyHeart[i].ProvinceName = province.nameWithType.ToString();
                        propertyHeart[i].ProvinceNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(province.nameWithType.ToString());
                        propertyHeart[i].Location = propertyHeart[i].Location + ", " + propertyHeart[i].ProvinceName;
                        propertyHeart[i].LocationEn = propertyHeart[i].LocationEn + ", " + propertyHeart[i].ProvinceNameEn;
                    }
                }

                // =================
                // MASTER DATA
                // =================
                // bedrooms
                if (propertyInfo.BedroomId != null)
                {
                    var bedroomTypes = JObject.Parse(File.ReadAllText(masterData))["bedroomTypes"].Where(n => n["id"].Value<string>() == propertyInfo.BedroomId);
                    if (bedroomTypes.ToList().Count > 0)
                    {
                        propertyHeart[i].BedroomId = propertyInfo.BedroomId;
                        propertyHeart[i].BedroomVi = bedroomTypes.Select(n => new { Value = n["bedroomTypesVi"] }).FirstOrDefault().Value.ToString();
                        propertyHeart[i].BedroomEn = bedroomTypes.Select(n => new { Value = n["bedroomTypesEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // bathrooms
                if (propertyInfo.BedroomId != null)
                {
                    var bathroomTypes = JObject.Parse(File.ReadAllText(masterData))["bathroomTypes"].Where(n => n["id"].Value<string>() == propertyInfo.BathroomId);
                    if (bathroomTypes.ToList().Count > 0)
                    {
                        propertyHeart[i].BedroomId = propertyInfo.BedroomId;
                        propertyHeart[i].BathroomVi = bathroomTypes.Select(n => new { Value = n["bathroomTypesVi"] }).FirstOrDefault().Value.ToString();
                        propertyHeart[i].BathroomEn = bathroomTypes.Select(n => new { Value = n["bathroomTypesEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // property type
                if (propertyInfo.PropertyTypeId != null)
                {
                    var propertyServiceType = JObject.Parse(File.ReadAllText(masterData))["propertyType"].Where(n => n["id"].Value<string>() == propertyInfo.PropertyTypeId);
                    if (propertyServiceType.ToList().Count > 0)
                    {
                        propertyHeart[i].PropertyTypeId = propertyInfo.PropertyTypeId;
                        propertyHeart[i].PropertyTypeVi = propertyServiceType.Select(n => new { Value = n["propertyTypeVi"] }).FirstOrDefault().Value.ToString();
                        propertyHeart[i].PropertyTypeEn = propertyServiceType.Select(n => new { Value = n["propertyTypeEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // transaction type
                if (propertyInfo.TransactionTypeId != null)
                {
                    var transactionType = JObject.Parse(File.ReadAllText(masterData))["transactionType"].Where(n => n["id"].Value<string>() == propertyInfo.TransactionTypeId);
                    if (transactionType.ToList().Count > 0)
                    {
                        propertyHeart[i].TransactionTypeId = propertyInfo.TransactionTypeId;
                        propertyHeart[i].TransactionTypeVi = transactionType.Select(n => new { Value = n["transactionTypeVi"] }).FirstOrDefault().Value.ToString();
                        propertyHeart[i].TransactionTypeEn = transactionType.Select(n => new { Value = n["transactionTypeEn"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // currency
                if (propertyInfo.CurrencyId != null)
                {
                    var currency = JObject.Parse(File.ReadAllText(masterData))["currency"].Where(n => n["id"].Value<string>() == propertyInfo.CurrencyId);
                    if (currency.ToList().Count > 0)
                    {
                        propertyHeart[i].CurrencyId = propertyInfo.CurrencyId;
                        propertyHeart[i].CurrencyName = currency.Select(n => new { Value = n["currencyName"] }).FirstOrDefault().Value.ToString();
                        propertyHeart[i].CurrencyNotation = currency.Select(n => new { Value = n["notation"] }).FirstOrDefault().Value.ToString();
                    }
                }

                propertyHeart[i].Title = propertyInfo.Title;
                propertyHeart[i].PropertyNumber = propertyInfo.PropertyNumber;
                propertyHeart[i].CoverImage = propertyInfo.CoverImage;
                propertyHeart[i].CoverImageUrl = !string.IsNullOrEmpty(propertyInfo.CoverImage) ? host + propertyInfo.CoverImage : "";
                propertyHeart[i].PropertyAddressVi = propertyInfo.PropertyAddressVi;
                propertyHeart[i].PropertyAddressEn = propertyInfo.PropertyAddressEn;
                propertyHeart[i].Price = propertyInfo.Price;
                propertyHeart[i].LotSize = propertyInfo.LotSize;
                propertyHeart[i].Longitude = propertyInfo.Longitude;
                propertyHeart[i].Latitude = propertyInfo.Latitude;
                propertyHeart[i].ApproveDate = propertyInfo.ApproveDate;
                propertyHeart[i].CreateTime = propertyHeart[i].CreateTime;
                propertyHeart[i].LotSizeFeet = Decimal.Round((propertyInfo.LotSize * (decimal)10.76391) ?? 0, 0);
                propertyHeart[i].USDPrice = propertyInfo.USDPrice;
                propertyHeart[i].Slug = propertyInfo.Slug;

                result.Add(propertyHeart[i]);
            }

            if (model.SortingModel != null)
            {
                // Price
                if (model.SortingModel.LowestPrice == true)
                {
                    result = result
                        .OrderBy(x => x.Price)
                        .ToList();
                }

                if (model.SortingModel.HighestPrice == true)
                {
                    result = result
                        .OrderByDescending(x => x.Price)
                        .ToList();
                }

                // Property Date
                if (model.SortingModel.Oldest == true)
                {
                    result = result
                        .OrderBy(x => x.ApproveDate)
                        .ToList();
                }

                if (model.SortingModel.Newest == true)
                {
                    result = result
                        .OrderByDescending(x => x.ApproveDate)
                        .ToList();
                }

                // Favourite Date
                if (model.SortingModel.OldFavourite == true)
                {
                    result = result
                        .OrderBy(x => x.CreateTime)
                        .ToList();
                }

                if (model.SortingModel.NewFavourite == true)
                {
                    result = result
                        .OrderByDescending(x => x.CreateTime)
                        .ToList();
                }
            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<ListPropertyHeartModel>(result, result.Count, 1, result.Count);
            }

            var paginatedList = PaginatedList<ListPropertyHeartModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);
            return paginatedList;
        }
    }
}
