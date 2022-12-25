using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Enums;
using General.Application.Common.Results;
using General.Application.Interfaces;
using General.Domain.Enums;
using General.Domain.Models;
using General.Domain.Models.ProjectElementModels;
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

namespace General.Application.Project.Queries
{
    public class GetProjectByIdQuery : IRequest<ProjectModel>
    {
        public Guid Id { set; get; }
    }

    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;
        private readonly IIdentityService _identityService;
        private readonly IConvertVietNameseService _convertVietnameseService;

        public GetProjectByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICommonFunctionService commonFunctionService,
            IIdentityService identityService,
            IConvertVietNameseService convertVietnameseService
        ) {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _convertVietnameseService = convertVietnameseService ?? throw new ArgumentNullException(nameof(convertVietnameseService));
        }

        public async Task<ProjectModel> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var result = await _context.Project
                            .Where(x => x.IsDeleted == DeletedStatus.False && x.Id == request.Id)
                            .AsNoTracking()
                            .ProjectTo<ProjectModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();
            var projectViewCount = await _context.ProjectViewCount
                    .Where(x => x.ProjectId == result.Id)
                    .Select(x => new {
                        ViewCount = x.ViewCount,
                    })
                    .FirstOrDefaultAsync();

            var masterData = "MasterData.json";
            var mapJSON = "map.json";

            result.CoverImageUrl = !string.IsNullOrEmpty(result.CoverImage) ? host + result.CoverImage : "";
            result.ProjectLogoUrl = !string.IsNullOrEmpty(result.ProjectLogo) ? host + result.ProjectLogo : "";
            result.MapViewImageUrl = !string.IsNullOrEmpty(result.MapViewImage) ? host + result.MapViewImage : "";
            result.ViewCount = projectViewCount != null ? projectViewCount.ViewCount : 0;

            if (result.Status == ProjectStatus.AlmostSoldOut)
            {
                result.StatusName = "almostSoldOut";
            }
            else if (result.Status == ProjectStatus.CommingSoon)
            {
                result.StatusName = "commingSoon";
            }
            else if (result.Status == ProjectStatus.OpenForSale)
            {
                result.StatusName = "openForSale";
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
            // DB DATA
            // =================
            // Seller
            var projectSeller = _context.ProjectSeller.Where(x => x.ProjectId == result.Id).ProjectTo<ProjectSellerModel>(_mapper.ConfigurationProvider).ToList();
            if (projectSeller != null && projectSeller.Count() > 0)
            {
                result.ProjectSellers = projectSeller;

                var sellers = _context.ProfileInformation.ToList();
                if (sellers != null)
                {
                    var socialNetworkUsersEntity = await _context.SocialNetworkUser
                        .ProjectTo<SocialNetworkUserModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                    for (int i = 0; i < result.ProjectSellers.Count(); i++)
                    {
                        for (int j = 0; j < sellers.Count(); j++)
                        {
                            if (result.ProjectSellers[i].UserId == sellers[j].UserId)
                            {
                                var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(sellers[j].UserId.ToString()));
                                var socialNetworkUsers = socialNetworkUsersEntity
                                                .Where(x => x.ProfileId == sellers[j].Id).ToList();

                                if (socialNetworkUsers.Count() > 0)
                                {
                                    var socialNetworks = await _context.SocialNetwork.ToListAsync();

                                    for (int x = 0; x < socialNetworkUsers.Count(); x++)
                                    {
                                        string icon = socialNetworks.Where(b => b.Id == socialNetworkUsers[x].SocialNetworkId).FirstOrDefault().ICon;
                                        socialNetworkUsers[x].SocialNetworkIconUrl = !string.IsNullOrEmpty(icon) ? host + icon : "";
                                    }
                                }

                                result.ProjectSellers[i].Avatar = sellers[j].Avatar;
                                result.ProjectSellers[i].AvatarUrl = !string.IsNullOrEmpty(sellers[j].Avatar) ? host + sellers[j].Avatar : "";
                                result.ProjectSellers[i].Email = user != null ? user.Email : "";
                                result.ProjectSellers[i].FirstName = sellers[j].FirstName;
                                result.ProjectSellers[i].LastName = sellers[j].LastName;
                                result.ProjectSellers[i].PhoneNumber1 = sellers[j].PhoneNumber1;
                                result.ProjectSellers[i].PhoneNumber2 = sellers[j].PhoneNumber2;
                                result.ProjectSellers[i].PhoneNumber3 = sellers[j].PhoneNumber3;
                                result.ProjectSellers[i].TitleVi = sellers[j].TitleVi;
                                result.ProjectSellers[i].TitleEn = sellers[j].TitleEn;
                                result.ProjectSellers[i].TitleDescriptionVi = sellers[j].TitleDescriptionVi;
                                result.ProjectSellers[i].TitleDescriptionEn = sellers[j].TitleDescriptionEn;
                                result.ProjectSellers[i].SocialNetworkUsers = socialNetworkUsers;

                                break;
                            }
                        }
                    }
                }
            }

            // project image
            var projectImage = _context.ProjectImage.Where(x => x.ProjectId == result.Id).ProjectTo<ProjectImageModel>(_mapper.ConfigurationProvider).ToList();
            if (projectImage != null)
            {
                result.ProjectImages = projectImage;

                for (int i = 0; i < result.ProjectImages.Count(); i++)
                {
                    result.ProjectImages[i].ImagesPathUrl = !string.IsNullOrEmpty(result.ProjectImages[i].ImagesPath) ? host + result.ProjectImages[i].ImagesPath : "";
                }
            }

            // =================
            // MASTER DATA
            // =================
            // Project - Feature
            var projectFeature = _context.ProjectFeature.Where(x => x.ProjectId == result.Id).ProjectTo<ProjectFeatureModel>(_mapper.ConfigurationProvider).ToList();
            if (projectFeature != null && projectFeature.Count() > 0)
            {
                result.ProjectFeatures = projectFeature;

                var view = JObject.Parse(File.ReadAllText(masterData))["projectFeature"].ToArray();
                if (view != null)
                {
                    for (int i = 0; i < result.ProjectFeatures.Count(); i++)
                    {
                        for (int j = 0; j < view.Count(); j++)
                        {
                            if (result.ProjectFeatures[i].ProjectFeatureId == view[j].Value<string>("id").ToString())
                            {
                                result.ProjectFeatures[i].ProjectFeatureVi = view[j].Value<string>("projectFeatureVi").ToString();
                                result.ProjectFeatures[i].ProjectFeatureEn = view[j].Value<string>("projectFeatureEn").ToString();

                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
