using AutoMapper;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Enums;
using General.Domain.Models;
using General.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Infrastructure.Services
{
    public class HandlePropertyService : IHandlePropertyService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IConvertVietNameseService _convertVietnameseService;

        public HandlePropertyService(
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IConvertVietNameseService convertVietnameseService
        )
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _convertVietnameseService = convertVietnameseService ?? throw new ArgumentNullException(nameof(convertVietnameseService));
        }

        public async Task<string> GeneratePropertyNumber(string transactionType)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                string numberCount = "";

                string date = DateTime.Now.ToString("yyyy");

                var propertyNumberCounter = await _context.PropertyNumberCounter
                                .Where(x => x.TransactionType == transactionType 
                                    && x.Date.Date.Year == DateTime.Now.Year
                                    && x.Date.Date.Month == DateTime.Now.Month)
                                .FirstOrDefaultAsync();

                if (propertyNumberCounter == null)
                {
                    numberCount = "0001";
                    PropertyNumberCounterEntity entity = new PropertyNumberCounterEntity()
                    {
                        Id = Guid.NewGuid(),
                        Date = DateTime.Now,
                        TransactionType = transactionType,
                        CurValue = 1
                    };

                    _context.PropertyNumberCounter.Add(entity);
                }
                else
                {
                    propertyNumberCounter.CurValue = propertyNumberCounter.CurValue + 1;
                    if (propertyNumberCounter.CurValue < 10)
                    {
                        numberCount = "000" + propertyNumberCounter.CurValue.ToString();
                    }
                    else if (propertyNumberCounter.CurValue >= 10 && propertyNumberCounter.CurValue < 100)
                    {
                        numberCount = "00" + propertyNumberCounter.CurValue.ToString();
                    }
                    else if (propertyNumberCounter.CurValue >= 100 && propertyNumberCounter.CurValue < 1000)
                    {
                        numberCount = "0" + propertyNumberCounter.CurValue.ToString();
                    }
                    else if (propertyNumberCounter.CurValue >= 1000 && propertyNumberCounter.CurValue < 10000)
                    {
                        numberCount = propertyNumberCounter.CurValue.ToString();
                    }
                }

                var propertyNumber = transactionType + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + numberCount;
                await _context.SaveChangesAsync(new CancellationToken());

                return propertyNumber;
            }
        }

        public async Task<List<ListPropertyModel>> JoinPropertyElements(List<ListPropertyModel> list)
        {
            var mapData = JObject.Parse(File.ReadAllText("map.json"));
            var masterData = JObject.Parse(File.ReadAllText("MasterData.json"));

            string host = _commonFunctionService.ConvertImageUrl("");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var provincesList = mapData["cities"].Select(n => new { code = n["code"], name = n["name"], nameWithType = n["nameWithType"] }).ToArray();
                var districtsList = mapData["districts"].Select(n => new { code = n["code"], name = n["name"], nameWithType = n["nameWithType"] }).ToArray();
                var wardsList = mapData["wards"].Select(n => new { code = n["code"], name = n["name"], nameWithType = n["nameWithType"] }).ToArray();

                List<Guid> listId = list.Select(x => x.Id).ToList();
                var listPropertyViewCount = await _context.PropertyViewCount.Where(x => listId.Contains(x.PropertyId)).AsNoTracking().ToListAsync();

                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].CoverImageUrl = !string.IsNullOrEmpty(list[i].CoverImage) ? host + list[i].CoverImage : "";
                    if (list[i].IsTemp)
                    {
                        list[i].IsApprove = PropertyApproveStatus.Draft;
                    }
                    list[i].ApproveStatusName = list[i].IsApprove.ToString();

                    var propertyViewCount = listPropertyViewCount.Where(x => x.PropertyId == list[i].Id).FirstOrDefault();
                    list[i].ViewCount = 0;
                    if (propertyViewCount != null)
                    {
                        list[i].ViewCount = propertyViewCount.ViewCount;
                    }

                    list[i].LotSizeFeet = Decimal.Round((list[i].LotSize * (decimal)10.76391) ?? 0, 0);

                    // Time For Post
                    if ((list[i].TimeForPostId != null || list[i].TimeForPostId != Guid.Empty) && list[i].IsApprove == PropertyApproveStatus.Active)
                    {
                        var timeForPost = _context.TimeForPost.Where(x => x.Id == list[i].TimeForPostId).FirstOrDefault();

                        if (timeForPost != null)
                        {
                            list[i].TimeForPostValue = timeForPost.Value;
                            list[i].TimeForPostName = timeForPost.DisplayName;

                            // calcu time for post
                            if (list[i].ApproveDate != null)
                            {
                                DateTime postingStartDate = DateTime.Parse(list[i].ApproveDate.ToString());
                                DateTime postingExpiryDate = postingStartDate.AddDays(Convert.ToDouble(timeForPost.Value));

                                list[i].TimeRemain = Math.Round(Convert.ToDecimal((postingExpiryDate - DateTime.Now).TotalDays), 0);
                                if (list[i].TimeRemain < 0)
                                {
                                    list[i].TimeRemain = 0;
                                }
                            }
                        }
                    }

                    // =================
                    // ADMINISTRATIVE
                    // =================
                    // ward
                    if (string.IsNullOrEmpty(list[i].WardCode) == false)
                    {
                        var ward = wardsList.Where(x => x.code.ToString() == list[i].WardCode).FirstOrDefault();
                        if (ward != null)
                        {
                            list[i].WardName = ward.nameWithType.ToString();
                            list[i].WardNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(ward.nameWithType.ToString());
                            list[i].Location = list[i].WardName + ", ";
                            list[i].LocationEn = list[i].WardNameEn + ", ";
                        }
                    }

                    // district
                    if (string.IsNullOrEmpty(list[i].DistrictCode) == false)
                    {
                        var district = districtsList.Where(x => x.code.ToString() == list[i].DistrictCode).FirstOrDefault();
                        if (district != null)
                        {
                            list[i].DistrictName = district.nameWithType.ToString();
                            list[i].DistrictNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(district.nameWithType.ToString());
                            list[i].Location += list[i].DistrictName + ", ";
                            list[i].LocationEn += list[i].DistrictNameEn + ", ";
                        }
                    }

                    // province
                    if (string.IsNullOrEmpty(list[i].ProvinceCode) == false)
                    {
                        var province = provincesList.Where(x => x.code.ToString() == list[i].ProvinceCode).FirstOrDefault();
                        if (province != null)
                        {
                            list[i].ProvinceName = province.name.ToString();
                            list[i].ProvinceNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(province.nameWithType.ToString(), true);
                            list[i].Location += list[i].ProvinceName;
                            list[i].LocationEn += list[i].ProvinceNameEn;
                        }
                    }

                    // =================
                    // MASTER DATA
                    // =================
                    // bedrooms
                    if (list[i].BedroomId != null)
                    {
                        var bedroomTypes = masterData["bedroomTypes"].Where(n => n["id"].Value<string>() == list[i].BedroomId);
                        if (bedroomTypes.ToList().Count > 0)
                        {
                            list[i].BedroomVi = bedroomTypes.Select(n => new { Value = n["bedroomTypesVi"] }).FirstOrDefault().Value.ToString();
                            list[i].BedroomEn = bedroomTypes.Select(n => new { Value = n["bedroomTypesEn"] }).FirstOrDefault().Value.ToString();
                        }
                    }

                    // bathrooms
                    if (list[i].BathroomId != null)
                    {
                        var bathroomTypes = masterData["bathroomTypes"].Where(n => n["id"].Value<string>() == list[i].BathroomId);
                        if (bathroomTypes.ToList().Count > 0)
                        {
                            list[i].BathroomVi = bathroomTypes.Select(n => new { Value = n["bathroomTypesVi"] }).FirstOrDefault().Value.ToString();
                            list[i].BathroomEn = bathroomTypes.Select(n => new { Value = n["bathroomTypesEn"] }).FirstOrDefault().Value.ToString();
                        }
                    }

                    // property type
                    if (list[i].PropertyTypeId != null)
                    {
                        var propertyServiceType = masterData["propertyType"].Where(n => n["id"].Value<string>() == list[i].PropertyTypeId);
                        if (propertyServiceType.ToList().Count > 0)
                        {
                            list[i].PropertyTypeVi = propertyServiceType.Select(n => new { Value = n["propertyTypeVi"] }).FirstOrDefault().Value.ToString();
                            list[i].PropertyTypeEn = propertyServiceType.Select(n => new { Value = n["propertyTypeEn"] }).FirstOrDefault().Value.ToString();
                        }
                    }

                    // transaction type
                    if (list[i].TransactionTypeId != null)
                    {
                        var transactionType = masterData["transactionType"].Where(n => n["id"].Value<string>() == list[i].TransactionTypeId);
                        if (transactionType.ToList().Count > 0)
                        {
                            list[i].TransactionTypeVi = transactionType.Select(n => new { Value = n["transactionTypeVi"] }).FirstOrDefault().Value.ToString();

                            list[i].TransactionTypeEn = transactionType.Select(n => new { Value = n["transactionTypeEn"] }).FirstOrDefault().Value.ToString();
                        }
                    }

                    // currency
                    if (list[i].CurrencyId != null)
                    {
                        var currency = masterData["currency"].Where(n => n["id"].Value<string>() == list[i].CurrencyId);
                        if (currency.ToList().Count > 0)
                        {
                            list[i].CurrencyName = currency.Select(n => new { Value = n["currencyName"] }).FirstOrDefault().Value.ToString();

                            list[i].CurrencyNotation = currency.Select(n => new { Value = n["notation"] }).FirstOrDefault().Value.ToString();
                        }
                    }
                }
            }


            return list;
        }
    }
}
