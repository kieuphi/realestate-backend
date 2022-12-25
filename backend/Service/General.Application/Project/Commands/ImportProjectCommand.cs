using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using AutoMapper;
using General.Application.Interfaces;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using General.Domain.Enums;

namespace General.Application.Project.Commands
{
    public class ImportProjectCommand : IRequest<List<Result>>
    {
        public List<ImportProjectModel> ImportModel { set; get; }
    }

    public class ImportProjectCommandHandler : IRequestHandler<ImportProjectCommand, List<Result>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _user;

        public ImportProjectCommandHandler(
            IMapper mapper,
            IApplicationDbContext context,
            ICurrentUserService user
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public async Task<List<Result>> Handle(ImportProjectCommand request, CancellationToken cancellationToken)
        {
            List<Result> results = new List<Result>();
            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            // database
            var project = await _context.Project.ToListAsync();

            // master data
            var propertyTypeList = JObject.Parse(File.ReadAllText(masterData))["propertyType"].ToList();
            var projectFeatureList = JObject.Parse(File.ReadAllText(masterData))["projectFeature"].ToList();
            var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToList();
            var districtsList = JObject.Parse(File.ReadAllText(mapJSON))["districts"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"], parentCode = n["parentCode"] }).ToList();
            var wardsList = JObject.Parse(File.ReadAllText(mapJSON))["wards"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"], parentCode = n["parentCode"] }).ToList();

            var importModel = _mapper.Map<List<ImportProjectResultModel>>(request.ImportModel);
            foreach (var item in importModel)
            {
                List<string> errorMessage = new List<string>();

                // =============
                // Check empty
                // =============
                // ProjectVi
                if (string.IsNullOrEmpty(item.ProjectVi))
                {
                    errorMessage.Add("Project title VI is required");
                } else
                {
                    if (item.ProjectVi.Length > 150)
                    {
                        errorMessage.Add("Project title VI can not be over 150 characters");
                    }
                }

                // ProjectEn
                if (string.IsNullOrEmpty(item.ProjectEn))
                {
                    errorMessage.Add("Project title EN is required");
                } else
                {
                    if (item.ProjectEn.Length > 150)
                    {
                        errorMessage.Add("Project title EN can not be over 150 characters");
                    }
                }

                // Longtitude
                if (string.IsNullOrEmpty(item.Longtitude))
                {
                    errorMessage.Add("Longitude is required");
                }

                // Latitude
                if (string.IsNullOrEmpty(item.Latitude))
                {
                    errorMessage.Add("Latitude is required");
                }

                // =============
                // Check wrong type
                // =============
                if (!string.IsNullOrEmpty(item.StartDate))
                {
                    if (int.TryParse(item.StartDate, out int c) == false)
                    {
                        errorMessage.Add("Start Date must be a number of year");
                    }
                    else
                    {
                        if (Int32.Parse(item.StartDate) <= 0)
                        {
                            errorMessage.Add("Start Date must be greater than 0");
                        }
                        else
                        {
                            if (item.StartDate.Length < 4)
                            {
                                errorMessage.Add("Start Date must be follow the format YYYY");
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.EndDate))
                {
                    if (int.TryParse(item.EndDate, out int c) == false)
                    {
                        errorMessage.Add("Estimate Completetion Date must be a number of year");
                    }
                    else
                    {
                        if (Int32.Parse(item.EndDate) <= 0)
                        {
                            errorMessage.Add("Estimate Completetion Date must be greater than 0");
                        }
                        else
                        {
                            if (item.EndDate.Length < 4)
                            {
                                errorMessage.Add("Estimate Completetion Date must be follow the format YYYY");
                            }
                        }
                    }
                }

                // =============
                // Check empty and valid value
                // =============
                // Property Type
                if (string.IsNullOrEmpty(item.PropertyTypeName))
                {
                    errorMessage.Add("Property Type is required");
                } else
                {
                    if (propertyTypeList.Count() == 0 || propertyTypeList.Count() > 0 &&
                        propertyTypeList.Where(n => n["propertyTypeEn"].Value<string>().Trim().ToLower() == item.PropertyTypeName.Trim().ToLower()).FirstOrDefault() == null)
                    {
                        errorMessage.Add("Property Type is invalid");
                    } else
                    {
                        item.PropertyTypeId = propertyTypeList
                            .Where(n => n["propertyTypeEn"].Value<string>().Trim().ToLower() == item.PropertyTypeName.Trim().ToLower())
                            .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                    }
                }

                if (string.IsNullOrEmpty(item.StatusName))
                {
                    errorMessage.Add("Project status is required");
                }
                else
                {
                    List<string> statusName = new List<string>(new string[] { "Coming Soon", "Open for Sale", "Almost Sold Out" });
                    if (statusName.ToList().Where(x => item.StatusName == x).Count() == 0)
                    {
                        errorMessage.Add("Project status is invalid");
                    } else
                    {
                        if (item.StatusName.Trim().ToLower() == ("Coming Soon").ToLower())
                        {
                            item.Status = ProjectStatus.CommingSoon;
                        } else if (item.StatusName.Trim().ToLower() == ("Open for Sale").ToLower())
                        {
                            item.Status = ProjectStatus.OpenForSale;
                        } else if (item.StatusName.Trim().ToLower() == ("Almost Sold Out").ToLower())
                        {
                            item.Status = ProjectStatus.AlmostSoldOut;
                        }
                    }
                }

                // Features
                if (!string.IsNullOrEmpty(item.ProjectFeatures))
                {
                    List<string> itemFeature = item.ProjectFeatures.Split(';').ToList();
                    if (projectFeatureList.Count() == 0 || itemFeature.Count() == 0)
                    {
                        errorMessage.Add("Features is invalid");
                    }
                    else
                    {
                        List<string> featureNameError = new List<string>();
                        item.ProjectFeatureIds = new List<string>();
                        foreach (var feature in itemFeature.ToList())
                        {
                            var projectFeature = projectFeatureList.Where(n => n["projectFeatureEn"].Value<string>().Trim().ToLower() == feature.Trim().ToLower()).FirstOrDefault();
                            if (projectFeature == null)
                            {
                                featureNameError.Add(feature);
                            }
                            else
                            {
                                string featureId = projectFeatureList
                                    .Where(n => n["projectFeatureEn"].Value<string>().Trim().ToLower() == feature.Trim().ToLower())
                                    .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                                item.ProjectFeatureIds.Add(featureId);
                            }
                        }

                        if (featureNameError.Count() > 0)
                        {
                            string error = string.Join(", ", featureNameError) + " is not existed";
                            errorMessage.Add(error);
                        }
                    }
                }

                // Administrative
                // Province
                string provinceCode = "";
                if (string.IsNullOrEmpty(item.ProvinceName))
                {
                    errorMessage.Add("Province is required");
                }
                else
                {
                    if (provincesList.Count() == 0 || provincesList.Count() > 0 &&
                        provincesList.Where(n => n.nameWithType.ToString().Trim().ToLower() == item.ProvinceName.Trim().ToLower()).FirstOrDefault() == null)
                    {
                        errorMessage.Add("Province is invalid");
                    } 
                    else
                    {
                        provinceCode = provincesList
                            .Where(n => n.nameWithType.ToString().Trim().ToLower() == item.ProvinceName.Trim().ToLower())
                            .FirstOrDefault()
                            .code.ToString();
                        item.ProvinceCode = provinceCode;
                    }
                }

                // District
                string districtCode = "";
                if (string.IsNullOrEmpty(item.DistrictName))
                {
                    errorMessage.Add("District is required");
                }
                else
                {
                    if (districtsList.Count() > 0)
                    {
                        var districtByProvince = districtsList.Where(n => n.parentCode.ToString().Trim().ToLower() == provinceCode).ToList();
                        if (districtByProvince.Count() > 0
                            && districtByProvince.Where(n => n.nameWithType.ToString().Trim().ToLower() == item.DistrictName.Trim().ToLower()).FirstOrDefault() == null)
                        {
                            errorMessage.Add("District does not belong to Province");
                        }
                        else
                        {
                            var district = districtsList
                                .Where(n => n.nameWithType.ToString().Trim().ToLower() == item.DistrictName.Trim().ToLower())
                                .FirstOrDefault();
                            districtCode = district.code.ToString();
                            item.DistrictCode = districtCode;
                        }
                    }
                    else
                    {
                        errorMessage.Add("District is invalid");
                    }
                }

                // Ward
                if (!string.IsNullOrEmpty(item.WardName))
                {
                    if (wardsList.Count() > 0)
                    {
                        var wardByDistrict = wardsList.Where(n => n.parentCode.ToString().Trim().ToLower() == districtCode).ToList();
                        if (wardByDistrict.Count() > 0
                            && wardByDistrict.Where(n => n.nameWithType.ToString().Trim().ToLower() == item.WardName.Trim().ToLower()).FirstOrDefault() == null)
                        {
                            errorMessage.Add("Ward does not belong to District");
                        }
                        else
                        {
                            var ward = wardsList
                                .Where(n => n.nameWithType.ToString().Trim().ToLower() == item.WardName.Trim().ToLower())
                                .FirstOrDefault();
                            item.WardCode = ward.code.ToString();
                        }
                    }
                    else
                    {
                        errorMessage.Add("Ward is invalid");
                    }
                }

                //item.PostNo = (countNo + 1).ToString();

                if (errorMessage.Count() == 0)
                {
                    results.Add(Result.Success(item));
                }
                else
                {
                    results.Add(Result.Failure(errorMessage, item));
                }
            }

            return results;
        }
    }
}
