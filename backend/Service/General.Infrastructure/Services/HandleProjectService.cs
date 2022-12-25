using AutoMapper;
using General.Application.Interfaces;
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
using System.Threading.Tasks;

namespace General.Infrastructure.Services
{
    public class HandleProjectService : IHandleProjectService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IConvertVietNameseService _convertVietnameseService;

        public HandleProjectService(
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

        public async Task<List<ProjectModel>> JoinProjectElements(List<ProjectModel> list)
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

                var projectView = await _context.ProjectViewCount.ToListAsync();

                for (int i = 0; i < list.Count(); i++)
                {
                    list[i].CoverImageUrl = !string.IsNullOrEmpty(list[i].CoverImage) ? host + list[i].CoverImage : "";
                    list[i].ProjectLogoUrl = !string.IsNullOrEmpty(list[i].ProjectLogo) ? host + list[i].ProjectLogo : "";

                    var viewCountProject = projectView.Where(x => x.ProjectId == list[i].Id).FirstOrDefault();
                    list[i].ViewCount = viewCountProject != null ? viewCountProject.ViewCount : 0;

                    // Project Status
                    if (list[i].Status == ProjectStatus.AlmostSoldOut)
                    {
                        list[i].StatusName = "almostSoldOut";
                    }
                    else if (list[i].Status == ProjectStatus.CommingSoon)
                    {
                        list[i].StatusName = "commingSoon";
                    }
                    else if (list[i].Status == ProjectStatus.OpenForSale)
                    {
                        list[i].StatusName = "openForSale";
                    }

                    // Project Approve Stratus
                    if (list[i].IsApprove == ProjectApproveStatus.Active)
                    {
                        list[i].ApproveStatusName = "Active";
                    }
                    else if (list[i].IsApprove == ProjectApproveStatus.New)
                    {
                        list[i].ApproveStatusName = "New";
                    }
                    else if (list[i].IsApprove == ProjectApproveStatus.InActive)
                    {
                        list[i].ApproveStatusName = "Inactive";
                    }
                    else if (list[i].IsApprove == ProjectApproveStatus.Lock)
                    {
                        list[i].ApproveStatusName = "Lock";
                    }

                    // =================
                    // ADMINISTRATIVE
                    // =================
                    // ward
                    if (list[i].WardCode != null)
                    {
                        var ward = wardsList.Where(x => x.code.ToString() == list[i].WardCode).FirstOrDefault();
                        if (ward != null)
                        {
                            list[i].WardName = ward.nameWithType.ToString();
                            list[i].WardNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(ward.nameWithType.ToString());
                        }
                    }

                    // district
                    if (list[i].DistrictCode != null)
                    {
                        var district = districtsList.Where(x => x.code.ToString() == list[i].DistrictCode).FirstOrDefault();
                        if (district != null)
                        {
                            list[i].DistrictName = district.nameWithType.ToString();
                            list[i].DistrictNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(district.nameWithType.ToString());
                        }
                    }

                    // province
                    if (list[i].ProvinceCode != null)
                    {
                        var province = provincesList.Where(x => x.code.ToString() == list[i].ProvinceCode).FirstOrDefault();
                        if (province != null)
                        {
                            list[i].ProvinceName = province.name.ToString();
                            list[i].ProvinceNameEn = _convertVietnameseService.ConvertAdministrativedViToEn(province.nameWithType.ToString(), true);
                        }
                    }
                }

                return list;
            }
        }
    }
}
