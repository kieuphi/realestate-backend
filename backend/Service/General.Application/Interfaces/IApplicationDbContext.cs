using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using General.Domain.Entities;
using General.Domain.Entities.PropertyElementEntities;
using General.Domain.Entities.ProjectElementEntities;

namespace General.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
        int SaveChanges();
        DbSet<IdentityRole> Roles { get; }
        DbSet<ProfileInformationEntity> ProfileInformation { get; set; }
        DbSet<ActiveCodeInformationEntity> ActiveCodeInformation { get; set; }

        DbSet<NewsCategoryEntity> NewsCategory { set; get; }
        DbSet<NewsEntity> News { set; get; }
        DbSet<NewsViewCountEntity> NewsViewCount { get; set; }

        DbSet<PropertyEntity> Property { set; get; }
        DbSet<PropertyImageEntity> PropertyImage { set; get; }

        // Property Elements Context
        DbSet<PropertyViewEntity> PropertyView { set; get; }
        DbSet<PropertyAmenitiesNearbyEntity> PropertyAmenitiesNearby { set; get; }
        DbSet<PropertySellerEntity> PropertySeller { set; get; }
        DbSet<PropertyViewCountEntity> PropertyViewCount { set; get; }
        DbSet<AttachmentEntity> Attachment { get; set; }
        DbSet<AttachmentTypeEntity> AttachmentType { get; set; }
        DbSet<ImageCategoryEntity> ImageCategory { get; set; }

        DbSet<BannerEntity> Banner { get; set; }
        DbSet<ProjectEntity> Project { set; get; }
        DbSet<PropertyNumberCounterEntity> PropertyNumberCounter { set; get; }
        DbSet<ContactEntity> Contact{ set; get; }
        DbSet<TimeForPostEntity> TimeForPost { set; get; }
        DbSet<PropertyMeetingNoteEntity> PropertyMeetingNote { set; get; }
        //DbSet<SellerEntity> Seller { set; get; }
        DbSet<SocialNetworkEntity> SocialNetwork { set; get; }
        DbSet<SocialNetworkUserEntity> SocialNetworkUser { set; get; }
        //DbSet<SupplierEntity> Supplier { set; get; }
        DbSet<PropertyNearestEntity> PropertyNearest { set; get; }
        DbSet<PropertyFavoriteEntity> PropertyFavorite { set; get; }
        DbSet<PropertyHeartEntity> PropertyHeart { set; get; }


        // Project Element Context
        DbSet<ProjectFeatureEntity> ProjectFeature { set; get; }
        DbSet<ProjectImageEntity> ProjectImage { set; get; }
        DbSet<ProjectSellerEntity> ProjectSeller { set; get; }

        DbSet<ProjectViewCountEntity> ProjectViewCount { set; get; }
        DbSet<UserSavedSearchEntity> UserSavedSearch { set; get; }
        DbSet<NotificationEntity> Notification { set; get; }
        DbSet<UserNotificationRemoveEntity> UserNotificationRemove { set; get; }

        DbSet<UserNotificationSeenEntity> UserNotificationSeen { set; get; }
        DbSet<ConfigEntity> Config { set; get; }
    }
}
