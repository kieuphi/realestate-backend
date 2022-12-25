using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Shared.Models;
using AutoMapper;
using General.Application.Interfaces;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;

namespace General.Application.Property.Commands
{
    public class ImportPropertyCommand : IRequest<List<Result>>
    {
        public List<ImportPropertyModel> ImportModel { set; get; }
    }

    public class ImportPropertyCommandHandler : IRequestHandler<ImportPropertyCommand, List<Result>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _user;

        public ImportPropertyCommandHandler (
            IMapper mapper,
            IApplicationDbContext context,
            ICurrentUserService user
        ) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public async Task<List<Result>> Handle(ImportPropertyCommand request, CancellationToken cancellationToken)
        {
            List<Result> results = new List<Result>();
            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            // database
            var project = await _context.Project.ToListAsync();

            // master data
            var propertyTypeList = JObject.Parse(File.ReadAllText(masterData))["propertyType"].ToList();
            var transactionTypeList = JObject.Parse(File.ReadAllText(masterData))["transactionType"].ToList();
            var viewList = JObject.Parse(File.ReadAllText(masterData))["view"].ToList();
            var amentitiesList = JObject.Parse(File.ReadAllText(masterData))["amenitiesNearby"].ToList();
            var bedroomList = JObject.Parse(File.ReadAllText(masterData))["bedroomTypes"].ToList();
            var bathroomList = JObject.Parse(File.ReadAllText(masterData))["bathroomTypes"].ToList();
            var currencyList = JObject.Parse(File.ReadAllText(masterData))["currency"].ToList();
            var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"] }).ToList();
            var districtsList = JObject.Parse(File.ReadAllText(mapJSON))["districts"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"], parentCode = n["parentCode"] }).ToList();
            var wardsList = JObject.Parse(File.ReadAllText(mapJSON))["wards"]
                .Select(n => new { code = n["code"], nameWithType = n["nameWithType"], parentCode = n["parentCode"] }).ToList();

            var importModel = _mapper.Map<List<ImportPropertyResultModel>>(request.ImportModel);
            foreach (var item in importModel)
            {
                List<string> errorMessage = new List<string>();

                // =============
                // Check empty
                // =============
                // Address Vi
                if (string.IsNullOrEmpty(item.PropertyAddressVi))
                {
                    errorMessage.Add("Full address (Vi) is required");
                }

                // Address En
                if (string.IsNullOrEmpty(item.PropertyAddressEn))
                {
                    errorMessage.Add("Full address (En) is required");
                }

                // Price
                if (item.Price == null)
                {
                    errorMessage.Add("Price is required");
                }

                // Size
                if (item.LotSize == null)
                {
                    errorMessage.Add("Property size is required");
                }

                // Longtitude
                if (string.IsNullOrEmpty(item.Longitude))
                {
                    errorMessage.Add("Longitude is required");
                }

                // Latitude
                if (string.IsNullOrEmpty(item.Latitude))
                {
                    errorMessage.Add("Latitude is required");
                }

                if (string.IsNullOrEmpty(item.Descriptions))
                {
                    errorMessage.Add("Descriptions is required");
                }

                // =============
                // Check wrong type
                // =============
                if (decimal.TryParse(item.Price, out decimal a) == false)
                {
                    errorMessage.Add("Price must be a number");
                }

                if (decimal.TryParse(item.LotSize, out decimal b) == false)
                {
                    errorMessage.Add("Lotsize must be a number");
                }

                if (!string.IsNullOrEmpty(item.YearCompleted))
                {
                    if (int.TryParse(item.YearCompleted, out int c) == false)
                    {
                        errorMessage.Add("Year completed must be a number of year");
                    } else
                    {
                        if (Int32.Parse(item.YearCompleted) <= 0)
                        {
                            errorMessage.Add("Year completed must be greater than 0");
                        } else
                        {
                            if (item.YearCompleted.Length < 4)
                            {
                                errorMessage.Add("Year completed must be follow the format YYYY");
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.FloorsNumber) && decimal.TryParse(item.FloorsNumber, out decimal d) == false)
                {
                    errorMessage.Add("Floor number must be a number");
                }

                if (!string.IsNullOrEmpty(item.TotalBuildingFloors) && decimal.TryParse(item.TotalBuildingFloors, out decimal e) == false)
                {
                    errorMessage.Add("Total building floors must be a number");
                }

                // =============
                // Check empty and valid value
                // =============
                // Title
                if(string.IsNullOrEmpty(item.Title))
                {
                    errorMessage.Add("Title is required");
                } else
                {
                    if (item.Title.Length > 150)
                    {
                        errorMessage.Add("Title can not be over 150 characters");
                    }
                }

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

                // Transaction Type
                if (string.IsNullOrEmpty(item.TransactionTypeName))
                {
                    errorMessage.Add("Transaction Type is required");
                } else
                {
                    if (transactionTypeList.Count() == 0 || transactionTypeList.Count() > 0 &&
                      transactionTypeList.Where(n => n["transactionTypeEn"].Value<string>().Trim().ToLower() == item.TransactionTypeName.Trim().ToLower()).FirstOrDefault() == null)
                    {
                        errorMessage.Add("Transaction Type is invalid");
                    } else
                    {
                        item.TransactionTypeId = transactionTypeList
                            .Where(n => n["transactionTypeEn"].Value<string>().Trim().ToLower() == item.TransactionTypeName.Trim().ToLower())
                            .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // Currency
                if (string.IsNullOrEmpty(item.CurrencyNotation))
                {
                    errorMessage.Add("Currency is required");
                } else
                {
                    if (currencyList.Count() == 0 || currencyList.Count() > 0 &&
                        currencyList.Where(n => n["notation"].Value<string>().Trim().ToLower() == item.CurrencyNotation.Trim().ToLower()).FirstOrDefault() == null)
                    {
                        errorMessage.Add("Currency is invalid");
                    } else
                    {
                        item.CurrencyId = currencyList
                            .Where(n => n["notation"].Value<string>().Trim().ToLower() == item.CurrencyNotation.Trim().ToLower())
                            .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                    }
                }

                // Bedroom
                if (item.TransactionTypeName.Trim().ToLower() != ("Commercial For Rent").ToLower()
                    && item.TransactionTypeName.Trim().ToLower() != ("Commercial For Sale").ToLower())
                {
                    if (string.IsNullOrEmpty(item.BedroomName))
                    {
                        errorMessage.Add("Bedroom is required");
                    }
                    else
                    {
                        if (bedroomList.Count() == 0 || bedroomList.Count() > 0 &&
                            bedroomList.Where(n => n["bedroomTypesEn"].Value<string>().Trim().ToLower() == item.BedroomName.Trim().ToLower()).FirstOrDefault() == null)
                        {
                            errorMessage.Add("Bedroom is invalid");
                        }
                        else
                        {
                            item.BedroomId = bedroomList
                                .Where(n => n["bedroomTypesEn"].Value<string>().Trim().ToLower() == item.BedroomName.Trim().ToLower())
                                .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                        }
                    }
                } else
                {
                    item.BedroomName = null;
                }

                // Bathroom
                if (item.TransactionTypeName.Trim().ToLower() != ("Commercial For Rent").ToLower() 
                    && item.TransactionTypeName.Trim().ToLower() != ("Commercial For Sale").ToLower())
                {
                    if (string.IsNullOrEmpty(item.BathroomName))
                    {
                        errorMessage.Add("Bathroom is required");
                    } else
                    {
                        if (bathroomList.Count() == 0 || bathroomList.Count() > 0 &&
                            bathroomList.Where(n => n["bathroomTypesEn"].Value<string>().Trim().ToLower() == item.BathroomName.Trim().ToLower()).FirstOrDefault() == null)
                        {
                            errorMessage.Add("Bathroom is invalid");
                        } else
                        {
                            item.BathroomId = bathroomList
                                .Where(n => n["bathroomTypesEn"].Value<string>().Trim().ToLower() == item.BathroomName.Trim().ToLower())
                                .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                        }
                    }
                } else
                {
                    item.BathroomName = null;
                }

                // Project
                //if (string.IsNullOrEmpty(item.ProjectName))
                //{
                //    errorMessage.Add("Project is required");
                //}
                //else
                //{
                //    if (project.Count() == 0 || project.Count() > 0 &&
                //        project.Where(n => n.ProjectEn.Trim().ToLower() == item.ProjectName.Trim().ToLower()).FirstOrDefault() == null)
                //    {
                //        errorMessage.Add("Project is invalid");
                //    } else
                //    {
                //        item.ProjectId = project.Where(n => n.ProjectEn.Trim().ToLower() == item.ProjectName.Trim().ToLower()).FirstOrDefault().Id;
                //    }
                //}

                // View
                if (string.IsNullOrEmpty(item.PropertyViews))
                {
                    errorMessage.Add("View is required");
                } else
                {
                    List<string> itemView = item.PropertyViews.Split(';').ToList();
                    if (viewList.Count() == 0 || itemView.Count() == 0)
                    {
                        errorMessage.Add("View is invalid");
                    } else
                    {
                        List<string> viewNameError = new List<string>();
                        item.PropertyViewIds = new List<string>();
                        foreach(var view in itemView.ToList())
                        {
                            var propertyView = viewList.Where(n => n["viewEn"].Value<string>().Trim().ToLower() == view.Trim().ToLower()).FirstOrDefault();
                            if (propertyView == null)
                            {
                                viewNameError.Add(view);
                            } else
                            {
                                string viewId = viewList
                                    .Where(n => n["viewEn"].Value<string>().Trim().ToLower() == view.Trim().ToLower())
                                    .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                                item.PropertyViewIds.Add(viewId);
                            }
                        }

                        if (viewNameError.Count() > 0)
                        {
                            string error = string.Join(", ", viewNameError) + " is not existed";
                            errorMessage.Add(error);
                        }
                    }
                }

                // Amentities nearby
                if (!string.IsNullOrEmpty(item.PropertyAmenitiesNearbys))
                {
                    List<string> amenitiesNearby = item.PropertyAmenitiesNearbys.Split(';').ToList();
                    item.PropertyAmenitiesNearbyIds = new List<string>();
                    if (amentitiesList.Count() == 0 || amenitiesNearby.Count() == 0)
                    {
                        errorMessage.Add("Amentities nearby is invalid");
                    }
                    else
                    {
                        List<string> amentityNameError = new List<string>();
                        foreach (var amentity in amenitiesNearby.ToList())
                        {
                            var amentityNearby = amentitiesList.Where(n => n["amenitiesNearbyEn"].Value<string>().Trim().ToLower() == amentity.Trim().ToLower()).FirstOrDefault();
                            if (amentityNearby == null)
                            {
                                amentityNameError.Add(amentity);
                            }
                            else
                            {
                                string viewId = amentitiesList
                                    .Where(n => n["amenitiesNearbyEn"].Value<string>().Trim().ToLower() == amentity.Trim().ToLower())
                                    .Select(n => new { Value = n["id"] }).FirstOrDefault().Value.ToString();
                                item.PropertyAmenitiesNearbyIds.Add(viewId);
                            }
                        }

                        if (amentityNameError.Count() > 0)
                        {
                            string error = string.Join(", ", amentityNameError) + " is not existed";
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
                        } else
                        {
                            var district = districtsList
                                .Where(n => n.nameWithType.ToString().Trim().ToLower() == item.DistrictName.Trim().ToLower())
                                .FirstOrDefault();
                                districtCode = district.code.ToString();
                            item.DistrictCode = districtCode;
                        }
                    } else
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
                        } else
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
