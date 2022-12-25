using Common.Shared.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Interfaces;
using General.Domain.Common;
using General.Domain.Entities;
using System.Reflection;
using General.Domain.Entities.PropertyElementEntities;
using General.Domain.Entities.ProjectElementEntities;

namespace General.Infrastructure.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly IDateTime _dateTime;
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
            : base(options, operationalStoreOptions)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }
        
        public DbSet<ProfileInformationEntity> ProfileInformation { get; set; }
        public DbSet<ActiveCodeInformationEntity> ActiveCodeInformation { get; set; }

        public DbSet<NewsCategoryEntity> NewsCategory { get; set; }
        public DbSet<NewsEntity> News { get; set; }
        public DbSet<NewsViewCountEntity> NewsViewCount { get; set; }

        public DbSet<PropertyEntity> Property { get; set; }
        public DbSet<PropertyImageEntity> PropertyImage { get; set; }
        public DbSet<PropertyViewCountEntity> PropertyViewCount { set; get; }
        // Property Elements Context
        public DbSet<PropertyViewEntity> PropertyView { set; get; }
        public DbSet<PropertyAmenitiesNearbyEntity> PropertyAmenitiesNearby { set; get; }
        public DbSet<PropertyMeetingNoteEntity> PropertyMeetingNote { set; get; }
        public DbSet<PropertySellerEntity> PropertySeller { set; get; }

        public DbSet<AttachmentEntity> Attachment { get; set; }
        public DbSet<AttachmentTypeEntity> AttachmentType { get; set; }
        public DbSet<ImageCategoryEntity> ImageCategory { get; set; }

        public DbSet<BannerEntity> Banner { get; set; }
        public DbSet<ProjectEntity> Project { set; get; }
        public DbSet<PropertyNumberCounterEntity> PropertyNumberCounter{ set; get; }
        public DbSet<ContactEntity> Contact { set; get; }
        public DbSet<TimeForPostEntity> TimeForPost { set; get; }
        //public DbSet<SellerEntity> Seller { set; get; }
        public DbSet<SocialNetworkEntity> SocialNetwork { set; get; }
        public DbSet<SocialNetworkUserEntity> SocialNetworkUser { set; get; }
        //public DbSet<SupplierEntity> Supplier { set; get; }
        public DbSet<PropertyNearestEntity> PropertyNearest { set; get; }
        public DbSet<PropertyFavoriteEntity> PropertyFavorite { set; get; }
        public DbSet<PropertyHeartEntity> PropertyHeart { set; get; }

        // Project Element Context
        public DbSet<ProjectFeatureEntity> ProjectFeature { set; get; }
        public DbSet<ProjectImageEntity> ProjectImage { set; get; }
        public DbSet<ProjectSellerEntity> ProjectSeller { set; get; }

        public DbSet<ProjectViewCountEntity> ProjectViewCount { set; get; }
        public DbSet<UserSavedSearchEntity> UserSavedSearch { set; get; }
        public DbSet<NotificationEntity> Notification { set; get; }
        public DbSet<UserNotificationRemoveEntity> UserNotificationRemove { set; get; }

        public DbSet<UserNotificationSeenEntity> UserNotificationSeen { set; get; }
        public DbSet<ConfigEntity> Config { set; get; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (EntityEntry<ApplicationUser> entry in ChangeTracker.Entries<ApplicationUser>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.LastModifiedBy = _currentUserService.UserName;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserName;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                }
            }

            foreach (EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateBy = _currentUserService.UserName;
                        entry.Entity.CreateTime = _dateTime.Now;
                        entry.Entity.CurrentState = 1;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdateBy = _currentUserService.UserName;
                        entry.Entity.UpdateTime = _dateTime.Now;
                        entry.Entity.CurrentState = entry.Entity.CurrentState + 1;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            foreach (EntityEntry<ApplicationUser> entry in ChangeTracker.Entries<ApplicationUser>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.LastModifiedBy = _currentUserService.UserName;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserName;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                }
            }

            foreach (EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateBy = _currentUserService.UserName;
                        entry.Entity.CreateTime = _dateTime.Now;
                        entry.Entity.CurrentState = 1;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdateBy = _currentUserService.UserName;
                        entry.Entity.UpdateTime = _dateTime.Now;
                        entry.Entity.CurrentState = entry.Entity.CurrentState + 1;
                        break;
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
