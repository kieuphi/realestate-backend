using AutoMapper;
using Common.Shared.Enums;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Property.Queries
{
    public class SuggestPropertyQuery : IRequest<SuggestSearchPropertyModel>
    {
        public string Keyword { set; get; }
        public SuggestPropertyTypes? SuggestPropertyType { set; get; }
    }

    public class SuggestPropertyQueryHandler : IRequestHandler<SuggestPropertyQuery, SuggestSearchPropertyModel>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IConvertVietNameseService _convertVietNameseService;

        public SuggestPropertyQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IConvertVietNameseService convertVietNameseService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
        }

        public async Task<SuggestSearchPropertyModel> Handle(SuggestPropertyQuery request, CancellationToken cancellationToken)
        {
            if(!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = _convertVietNameseService.ConvertVietNamese(request.Keyword).ToLower().Trim();
                var mapJSON = "map.json";
                var result = new SuggestSearchPropertyModel();
                var properties = await _context.Property
                            .Where(x => x.IsApprove == PropertyApproveStatus.Active && x.IsDeleted == DeletedStatus.False)
                            .OrderByDescending(x => x.ApproveDate)
                            .Select(x => new {
                                Id = x.Id,
                                PropertyAddressEn = x.PropertyAddressEn,
                                PropertyAddressVi = x.PropertyAddressVi,
                                Street = x.Street,
                                ProvinceCode = x.ProvinceCode,
                                DistrictCode = x.DistrictCode,
                                PropertyNumber = x.PropertyNumber,
                                TransactionTypeId = x.TransactionTypeId
                            }).ToListAsync();

                if (request.SuggestPropertyType == SuggestPropertyTypes.Commercial)
                {
                    properties = properties.Where(x => x.TransactionTypeId == "004" || x.TransactionTypeId == "005").ToList();
                } else if (request.SuggestPropertyType == SuggestPropertyTypes.NotCommercial)
                {
                    properties = properties.Where(x => x.TransactionTypeId == "001" || x.TransactionTypeId == "002").ToList();
                }

                var provinces = JObject.Parse(File.ReadAllText(mapJSON))["cities"]
                            .Where(n => _convertVietNameseService.ConvertVietNamese(n["nameWithType"].Value<string>().ToLower()).Contains(keyword) ||
                                _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].Value<string>()).ToLower().Contains(keyword))
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["name"].ToString())
                                //coordinates = n["coordinates"]
                            }).ToArray();

                var districts = JObject.Parse(File.ReadAllText(mapJSON))["districts"]
                            .Where(n => _convertVietNameseService.ConvertVietNamese(n["nameWithType"].Value<string>().ToLower()).Contains(keyword) ||
                                _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].Value<string>()).ToLower().Contains(keyword))
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["name"].ToString())
                                //coordinates = n["coordinates"]
                            }).ToArray();

                var suggestAdministrative = new List<SuggestPropertyAdministrativeModel>();
                var suggestProperty = new List<SuggestPropertyModel>();

                if (provinces != null && provinces.Count() > 0)
                {
                    foreach (var province in provinces)
                    {
                        var propertyExisted = properties.Where(x => x.ProvinceCode == province.code.ToString()).ToList();
                        if (propertyExisted.Count() > 0)
                        {
                            suggestAdministrative.Add(new SuggestPropertyAdministrativeModel
                            {
                                AdministrativeCode = province.code.ToString(),
                                AdministrativeName = province.name.ToString(),
                                AdministrativeNameEn = province.nameEn.ToString(),
                                //AdministrativeCoordinate = province.coordinates.ToString()
                            }); ;
                        }
                    }
                }

                if (districts != null && districts.Count() > 0)
                {
                    foreach (var district in districts)
                    {
                        var propertyExisted = properties.Where(x => x.DistrictCode == district.code.ToString()).ToList();
                        if (propertyExisted.Count() > 0)
                        {
                            suggestAdministrative.Add(new SuggestPropertyAdministrativeModel
                            {
                                AdministrativeCode = district.code.ToString(),
                                AdministrativeName = district.name.ToString(),
                                AdministrativeNameEn = district.nameEn.ToString(),
                                //AdministrativeCoordinate = district.coordinates.ToString()
                            });
                        }
                    }
                }

                var filterAdministrativeCode = suggestAdministrative != null && suggestAdministrative.Count() > 0 ? suggestAdministrative[0].AdministrativeCode : "";
                var propertiesExisted = properties.Where(x => x.PropertyNumber.ToLower().Contains(keyword.ToLower()) ||
                                            _convertVietNameseService.ConvertVietNamese(x.PropertyAddressEn).Contains(keyword) ||
                                            _convertVietNameseService.ConvertVietNamese(x.PropertyAddressVi).Contains(keyword) ||
                                            x.ProvinceCode == filterAdministrativeCode || x.DistrictCode == filterAdministrativeCode)
                                        .Take(5)
                                        .ToList();

                if (propertiesExisted.Count() > 0)
                {
                    foreach (var property in propertiesExisted)
                    {
                        //string address = property.PropertyAddressEn;
                        //var district = districts.Where(x => x.code.ToString() == property.DistrictCode).FirstOrDefault();
                        //var province = provinces.Where(x => x.code.ToString() == property.ProvinceCode).FirstOrDefault();

                        //if (district != null)
                        //{
                        //    address = address + ", " + district.name.ToString();
                        //}

                        //if (province != null)
                        //{
                        //    address = address + ", " + province.name.ToString();
                        //}

                        suggestProperty.Add(new SuggestPropertyModel
                        {
                            PropertyId = property.Id,
                            PropertyNumber = property.PropertyNumber,
                            Address = property.PropertyAddressEn != null ? property.PropertyAddressEn : property.PropertyAddressVi
                        });
                    }
                }

                result.SuggestAdministrative = suggestAdministrative;
                result.SuggestProperty = suggestProperty;

                return result;
            } else
            {
                var result = new SuggestSearchPropertyModel();

                return result;
            }
        }
    }
}
