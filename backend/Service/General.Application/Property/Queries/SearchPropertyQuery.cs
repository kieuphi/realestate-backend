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
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Property.Queries
{
    public class SearchPropertyQuery : IRequest<PropertyPaginatedList<ListPropertyModel>>
    {
        public SearchingPropertyModel SearchModel { set; get; }
    }

    public class SearchPropertyQueryHandler : IRequestHandler<SearchPropertyQuery, PropertyPaginatedList<ListPropertyModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<PropertyEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IConvertVietNameseService _convertVietNameseService;
        private readonly IHandlePropertyService _handlePropertyService;

        public SearchPropertyQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<PropertyEntity> repository,
            ICommonFunctionService commonFunctionService,
            IConvertVietNameseService convertVietNameseService,
            IHandlePropertyService handlePropertyService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
            _handlePropertyService = handlePropertyService ?? throw new ArgumentNullException(nameof(handlePropertyService));
        }

        public async Task<PropertyPaginatedList<ListPropertyModel>> Handle(SearchPropertyQuery request, CancellationToken cancellationToken)
        {
            var model = request.SearchModel;
            var mapData = JObject.Parse(File.ReadAllText("map.json"));
            var masterData = JObject.Parse(File.ReadAllText("MasterData.json"));

            var queryData = _context.Property
                .Where(x => x.IsDeleted == DeletedStatus.False && x.IsApprove == PropertyApproveStatus.Active)
                .AsNoTracking();

            string coordinates = "";

            if (!string.IsNullOrEmpty(model.PropertyKeyWord))
            {
                string administractiveCode = "";
                string vietnameseKeyword = !string.IsNullOrEmpty(model.PropertyKeyWord) ? _convertVietNameseService.ConvertVietNamese(model.PropertyKeyWord.ToLower().Trim()) : "";

                var provinces = mapData["cities"]
                            .Where(n => _convertVietNameseService.ConvertVietNamese(n["nameWithType"].Value<string>().ToLower()).Contains(vietnameseKeyword) ||
                                _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].Value<string>()).ToLower().Contains(vietnameseKeyword))
                            .Select(n => new {
                                code = n["code"],
                                //coordinates = n["coordinates"]
                            }).ToArray();

                var districts = mapData["districts"]
                            .Where(n => _convertVietNameseService.ConvertVietNamese(n["nameWithType"].Value<string>().ToLower()).Contains(vietnameseKeyword) ||
                                _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].Value<string>()).ToLower().Contains(vietnameseKeyword))
                            .Select(n => new {
                                code = n["code"],
                                //coordinates = n["coordinates"]
                            }).ToArray();

                if (provinces.Count() > 0)
                {
                    if (provinces.Count() == 1)
                    {
                        var province = mapData["cities"]
                                    .Where(n => n["code"].Value<string>().ToLower() == provinces[0].code.ToString())
                                    .Select(n => new {
                                        code = n["code"],
                                        coordinates = n["coordinates"]
                                    }).FirstOrDefault();

                        if (province != null)
                        {
                            coordinates = province.coordinates.ToString();
                            administractiveCode = provinces[0].code.ToString();

                            queryData = queryData.Where(x => x.ProvinceCode == administractiveCode);
                        }
                    } else
                    {
                        queryData = queryData.Where(x => provinces.Select(x => x.code.ToString()).Contains(x.ProvinceCode));
                    }
                } else if (districts.Count() > 0) 
                {
                    if (districts.Count() == 1)
                    {
                        var district = mapData["districts"]
                                   .Where(n => n["code"].Value<string>().ToLower() == districts[0].code.ToString())
                                   .Select(n => new {
                                       code = n["code"],
                                       coordinates = n["coordinates"]
                                   }).FirstOrDefault();

                        if (district != null)
                        {
                            coordinates = district.coordinates.ToString();
                            administractiveCode = districts[0].code.ToString();

                            queryData = queryData.Where(x => x.DistrictCode == administractiveCode);
                        }

                    }
                    else
                    {
                        queryData = queryData.Where(x => districts.Select(x => x.code.ToString()).Contains(x.DistrictCode));
                    }
                } else if (provinces.Count() == 0 && districts.Count() == 0)
                {
                    queryData = queryData.Where(x => x.PropertyNumber.ToLower().Contains(vietnameseKeyword));
                }
            }

            if(!string.IsNullOrEmpty(model.AdministrativeCode))
            {
                queryData = queryData.Where(p => p.ProvinceCode == model.AdministrativeCode || p.DistrictCode == model.AdministrativeCode);

                var province = mapData["cities"]
                           .Where(n => n["code"].Value<string>().ToLower() == model.AdministrativeCode)
                           .Select(n => new {
                               code = n["code"],
                               coordinates = n["coordinates"]
                           }).FirstOrDefault();

                var district = mapData["districts"]
                           .Where(n => n["code"].Value<string>().ToLower() == model.AdministrativeCode)
                           .Select(n => new {
                               code = n["code"],
                               coordinates = n["coordinates"]
                           }).FirstOrDefault();

                if (province != null)
                {
                    coordinates = province.coordinates.ToString();
                }

                if (district != null)
                {
                    coordinates = district.coordinates.ToString();
                }
            }

            if(!string.IsNullOrEmpty(model.PropertyTypeId))
            {
                queryData = queryData.Where(p => p.PropertyTypeId == model.PropertyTypeId);
            }

            // if length of transaction type ids > 0 , search with transactions ids
            if(!string.IsNullOrEmpty(model.TransactionTypeId))
            {
                if (model.TransactionTypeId == "004" && model.IsManyCommercialProperty == true)
                {
                    queryData = queryData.Where(p => p.TransactionTypeId == "004" || p.TransactionTypeId == "005");
                }
                else
                {
                    queryData = queryData.Where(p => p.TransactionTypeId == model.TransactionTypeId);
                }
            }

            if(model.MinPrice != null && model.MinPrice > 0)
            {
                queryData = queryData.Where(p => p.Price >= model.MinPrice);
            }

            if (model.MaxPrice != null && model.MaxPrice > 0)
            {
                queryData = queryData.Where(p => p.Price <= model.MaxPrice);
            }

            if (model.LandSize != null && model.LandSize > 0)
            {
                queryData = queryData.Where(p => p.LotSize >= model.LandSize);
            }

            if (!string.IsNullOrEmpty(model.BedroomId))
            {
                var bedroomTypes = masterData["bedroomTypes"].Where(n => n["id"].Value<string>() == model.BedroomId).FirstOrDefault();
                if(bedroomTypes != null)
                {
                    string[] includeOthersId = bedroomTypes["includeOthersId"].ToObject<string[]>();
                    if (includeOthersId.Length > 0)
                    {
                        queryData = queryData.Where(x => x.BedroomId != "" && (x.BedroomId == model.BedroomId || includeOthersId.Contains(x.BedroomId)));
                    }
                    else
                    {
                        queryData = queryData.Where(x => x.BedroomId != "" && x.BedroomId == model.BedroomId);
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.BathroomId))
            {
                var bathroomTypes = masterData["bathroomTypes"].Where(n => n["id"].Value<string>() == model.BathroomId).FirstOrDefault();
                if (bathroomTypes != null)
                {
                    string[] includeOthersId = bathroomTypes["includeOthersId"].ToObject<string[]>();
                    if (includeOthersId.Length > 0)
                    {
                        queryData = queryData.Where(x => x.BathroomId != "" && (x.BathroomId == model.BathroomId || includeOthersId.Contains(x.BathroomId)));
                    }
                    else
                    {
                        queryData = queryData.Where(x => x.BathroomId != "" && x.BathroomId == model.BathroomId);
                    }
                }
            }

            if (model.ListedSince != null && model.ListedSince != default(DateTime))
            {
                queryData = queryData.Where(p => p.ApproveDate >= model.ListedSince);
            }

            var finalQuery = queryData
                            .OrderByDescending(x => x.ApproveDate)
                            .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);

            if(model.SortingModel != null)
            {
                if (model.SortingModel.LowestPrice == true)
                {
                    finalQuery = queryData
                               .OrderBy(x => x.USDPrice)
                               .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.HighestPrice == true)
                {
                    finalQuery = queryData
                               .OrderByDescending(x => x.USDPrice)
                               .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.Oldest == true)
                {
                    finalQuery = queryData
                              .OrderBy(x => x.ApproveDate)
                              .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }

                if (model.SortingModel.Newest == true)
                {
                    finalQuery = queryData
                              .OrderByDescending(x => x.ApproveDate)
                              .ProjectTo<ListPropertyModel>(_mapper.ConfigurationProvider);
                }
            }

            var properties = await finalQuery.ToListAsync();

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                model.PageSize = properties.Count();
                model.PageNumber = 1;
            }

            var paginatedList = PropertyPaginatedList<ListPropertyModel>.Create(properties, coordinates, model.PageNumber.Value, model.PageSize.Value);
            paginatedList.Items = await _handlePropertyService.JoinPropertyElements(paginatedList.Items);

            return paginatedList;
        }
    }
}
