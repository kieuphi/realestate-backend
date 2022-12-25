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

namespace General.Application.PropertyNearest.Queries
{
    public class GetPagingPropertyNearestByUserIdQuery : IRequest<PaginatedList<ListPropertyNearestModel>>
    {
        public PagingPropertyNearestModel Model { set; get; }
    }

    public class GetPagingPropertyNearestByUserIdQueryHandler : IRequestHandler<GetPagingPropertyNearestByUserIdQuery, PaginatedList<ListPropertyNearestModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyNearestEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetPagingPropertyNearestByUserIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyNearestEntity> repository,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PaginatedList<ListPropertyNearestModel>> Handle(GetPagingPropertyNearestByUserIdQuery request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            var propertyNearest = await _context.PropertyNearest
                        .Where(x => x.IsDeleted == DeletedStatus.False && x.UserId == model.UserId)
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ListPropertyNearestModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            List<ListPropertyNearestModel> result = new List<ListPropertyNearestModel>();

            if (propertyNearest.Count() > 0)
            {
                var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
                var districtsList = JObject.Parse(File.ReadAllText(mapJSON))["districts"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();
                var wardsList = JObject.Parse(File.ReadAllText(mapJSON))["wards"].Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToArray();

                var properties = _context.Property.Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active).ToList();

                for (int i = 0; i < propertyNearest.Count(); i++)
                {
                    for(int j = 0; j < properties.Count(); j++)
                    {
                        if(propertyNearest[i].PropertyId == properties[j].Id)
                        {
                            var timeForPost = _context.TimeForPost.Where(x => x.Id == properties[j].TimeForPostId).FirstOrDefault();

                            propertyNearest[i].TimeForPostId = properties[j].TimeForPostId;
                            propertyNearest[i].TimeForPostValue = timeForPost.Value;
                            propertyNearest[i].TimeForPostName = timeForPost.DisplayName;

                            // calcu time for post
                            DateTime postingStartDate = DateTime.Parse(properties[j].ApproveDate.ToString());
                            DateTime postingExpiryDate = postingStartDate.AddDays(Convert.ToDouble(timeForPost.Value));

                            propertyNearest[i].TimeRemain = Math.Round(Convert.ToDecimal((postingExpiryDate - DateTime.Now).TotalDays), 0);

                            // =================
                            // ADMINISTRATIVE
                            // =================
                            // ward
                            if (properties[j].WardCode != null)
                            {
                                if (wardsList.Where(x => x.code.ToString() == properties[j].WardCode).FirstOrDefault() != null)
                                {
                                    propertyNearest[i].WardCode = properties[j].WardCode;
                                    propertyNearest[i].WardName = wardsList.Where(x => x.code.ToString() == properties[j].WardCode).FirstOrDefault().nameWithType.ToString();
                                    propertyNearest[i].Location = propertyNearest[i].WardName;
                                }
                            }

                            // district
                            if (properties[j].DistrictCode != null)
                            {
                                if (districtsList.Where(x => x.code.ToString() == properties[j].DistrictCode).FirstOrDefault() != null)
                                {
                                    propertyNearest[i].DistrictCode = properties[j].DistrictCode;
                                    propertyNearest[i].DistrictName = districtsList.Where(x => x.code.ToString() == properties[j].DistrictCode).FirstOrDefault().nameWithType.ToString();
                                    propertyNearest[i].Location = propertyNearest[i].Location + ", " + propertyNearest[i].DistrictName;
                                }
                            }

                            // province
                            if (properties[j].ProvinceCode != null)
                            {
                                if (provincesList.Where(x => x.code.ToString() == properties[j].ProvinceCode).FirstOrDefault() != null)
                                {
                                    propertyNearest[i].ProvinceCode = properties[j].ProvinceCode;
                                    propertyNearest[i].ProvinceName = provincesList.Where(x => x.code.ToString() == properties[j].ProvinceCode).FirstOrDefault().nameWithType.ToString();
                                    propertyNearest[i].Location = propertyNearest[i].Location + ", " + propertyNearest[i].ProvinceName;
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
                                    propertyNearest[i].BedroomId = properties[j].BedroomId;
                                    propertyNearest[i].BedroomVi = bedroomTypes.Select(n => new { Value = n["bedroomTypesVi"] }).FirstOrDefault().Value.ToString();
                                    propertyNearest[i].BedroomEn = bedroomTypes.Select(n => new { Value = n["bedroomTypesEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // bathrooms
                            if (properties[j].BedroomId != null)
                            {
                                var bathroomTypes = JObject.Parse(File.ReadAllText(masterData))["bathroomTypes"].Where(n => n["id"].Value<string>() == properties[j].BathroomId);
                                if (bathroomTypes.ToList().Count > 0)
                                {
                                    propertyNearest[i].BedroomId = properties[j].BedroomId;
                                    propertyNearest[i].BathroomVi = bathroomTypes.Select(n => new { Value = n["bathroomTypesVi"] }).FirstOrDefault().Value.ToString();
                                    propertyNearest[i].BathroomEn = bathroomTypes.Select(n => new { Value = n["bathroomTypesEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // property type
                            if (properties[j].PropertyTypeId != null)
                            {
                                var propertyServiceType = JObject.Parse(File.ReadAllText(masterData))["propertyType"].Where(n => n["id"].Value<string>() == properties[j].PropertyTypeId);
                                if (propertyServiceType.ToList().Count > 0)
                                {
                                    propertyNearest[i].PropertyTypeId = properties[j].PropertyTypeId;
                                    propertyNearest[i].PropertyTypeVi = propertyServiceType.Select(n => new { Value = n["propertyTypeVi"] }).FirstOrDefault().Value.ToString();
                                    propertyNearest[i].PropertyTypeEn = propertyServiceType.Select(n => new { Value = n["propertyTypeEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // transaction type
                            if (properties[j].TransactionTypeId != null)
                            {
                                var transactionType = JObject.Parse(File.ReadAllText(masterData))["transactionType"].Where(n => n["id"].Value<string>() == properties[j].TransactionTypeId);
                                if (transactionType.ToList().Count > 0)
                                {
                                    propertyNearest[i].TransactionTypeId = properties[j].TransactionTypeId;
                                    propertyNearest[i].TransactionTypeVi = transactionType.Select(n => new { Value = n["transactionTypeVi"] }).FirstOrDefault().Value.ToString();
                                    propertyNearest[i].TransactionTypeEn = transactionType.Select(n => new { Value = n["transactionTypeEn"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            // currency
                            if (properties[j].CurrencyId != null)
                            {
                                var currency = JObject.Parse(File.ReadAllText(masterData))["currency"].Where(n => n["id"].Value<string>() == properties[j].CurrencyId);
                                if (currency.ToList().Count > 0)
                                {
                                    propertyNearest[i].CurrencyId = properties[j].CurrencyId;
                                    propertyNearest[i].CurrencyName = currency.Select(n => new { Value = n["currencyName"] }).FirstOrDefault().Value.ToString();
                                    propertyNearest[i].CurrencyNotation = currency.Select(n => new { Value = n["notation"] }).FirstOrDefault().Value.ToString();
                                }
                            }

                            propertyNearest[i].Title = properties[j].Title;
                            propertyNearest[i].PropertyNumber = properties[j].PropertyNumber;
                            propertyNearest[i].CoverImage = properties[j].CoverImage;
                            propertyNearest[i].PropertyAddressVi = properties[j].PropertyAddressVi;
                            propertyNearest[i].PropertyAddressEn = properties[j].PropertyAddressEn;
                            propertyNearest[i].Price = properties[j].Price;
                            propertyNearest[i].Longitude = properties[j].Longitude;
                            propertyNearest[i].Latitude = properties[j].Latitude;

                            result.Add(propertyNearest[i]);

                            break;
                        }
                    }
                }

            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<ListPropertyNearestModel>(result, result.Count, 1, result.Count);
            }

            var paginatedList = PaginatedList<ListPropertyNearestModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);
            return paginatedList;
        }
    }
}
