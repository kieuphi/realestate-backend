using Microsoft.AspNetCore.Identity;
using General.Application.Common.Results;
using General.Domain.Common;
using General.Domain.Entities;
using General.Domain.Models;
using General.Domain.Entities.PropertyElementEntities;
using General.Domain.Models.PropertyElementModels;
using General.Domain.Entities.ProjectElementEntities;
using General.Domain.Models.ProjectElementModels;

namespace General.Application.Common.Mappings
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResult>()
                .ReverseMap();
            CreateMap<IdentityRole, RoleModel>().ReverseMap();

            CreateMap<CreateUserModel, ProfileInformationEntity>()
                .ReverseMap();

            CreateMap<ProfileInformationEntity, ProfileInformationModel>().ReverseMap();

            CreateMap<NewsCategoryEntity, NewsCategoryModel>().ReverseMap();
            CreateMap<NewsCategoryEntity, ListNewsCategoryModel>().ReverseMap();
            CreateMap<NewsCategoryEntity, ListNewsGroupByCategoryModel>().ReverseMap();

            CreateMap<NewsEntity, NewsModel>().ReverseMap();
            CreateMap<NewsEntity, ListNewsModel>().ReverseMap();
            CreateMap<NewsViewCountEntity, NewsViewCountModel>().ReverseMap();

            CreateMap<PropertyEntity, PropertyModel>().ReverseMap();
            CreateMap<PropertyEntity, ListPropertyModel>().ReverseMap();
            CreateMap<ImportPropertyModel, ImportPropertyResultModel>().ReverseMap();

            CreateMap<PropertyImageEntity, PropertyImageModel>().ReverseMap();

            // Property Elements
            CreateMap<PropertyViewEntity, PropertyViewModel>().ReverseMap();
            CreateMap<PropertyAmenitiesNearbyEntity, PropertyAmenitiesNearbyModel>().ReverseMap();
            CreateMap<PropertyMeetingNoteEntity, PropertyMeetingNoteModel>().ReverseMap();
            CreateMap<PropertySellerEntity, PropertySellerModel>().ReverseMap();

            CreateMap<AttachmentEntity, AttachmentModel>().ReverseMap();
            CreateMap<AttachmentTypeEntity, AttachmentTypeModel>().ReverseMap();
            CreateMap<ImageCategoryEntity, ImageCategoryModel>().ReverseMap();

            CreateMap<BannerEntity, BannerModel>().ReverseMap();

            // Project
            CreateMap<ProjectEntity, ProjectModel>().ReverseMap();
            CreateMap<ImportProjectModel, ImportProjectResultModel>().ReverseMap();

            CreateMap<ContactEntity, ContactModel>().ReverseMap();
            CreateMap<TimeForPostEntity, TimeForPostModel>().ReverseMap();
            CreateMap<SocialNetworkEntity, SocialNetworkModel>().ReverseMap();
            CreateMap<SocialNetworkUserEntity, SocialNetworkUserModel>().ReverseMap();
            CreateMap<PropertyNearestEntity, ListPropertyNearestModel>().ReverseMap();
            CreateMap<PropertyFavoriteEntity, ListPropertyFavoriteModel>().ReverseMap();
            CreateMap<PropertyHeartEntity, ListPropertyHeartModel>().ReverseMap();
            CreateMap<PropertyHeartEntity, PropertyHeartModel>().ReverseMap();

            // Project Elements
            CreateMap<ProjectFeatureEntity, ProjectFeatureModel>().ReverseMap();
            CreateMap<ProjectImageEntity, ProjectImageModel>().ReverseMap();
            CreateMap<ProjectSellerEntity, ProjectSellerModel>().ReverseMap();

            CreateMap<ProjectViewCountEntity, ProjectViewCountModel>().ReverseMap();
            CreateMap<UserSavedSearchEntity, UserSavedSearchModel>().ReverseMap();
            CreateMap<NotificationEntity, NotificationModel>().ReverseMap();
            CreateMap<NotificationEntity, NotificationUserModel>().ReverseMap();

            CreateMap<ConfigEntity, ConfigModel>().ReverseMap();
        }
    }
}
