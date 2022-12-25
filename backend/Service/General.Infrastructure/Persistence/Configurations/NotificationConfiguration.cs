using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
    {
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasColumnType("nvarchar(200)").IsRequired();
            builder.Property(x => x.TitleVi).HasColumnType("nvarchar(200)");
            builder.Property(x => x.Content).HasColumnType("nvarchar(350)").IsRequired();
            builder.Property(x => x.ContentVi).HasColumnType("nvarchar(350)");
        }
    }
    public class UserNotificationRemoveConfiguration : IEntityTypeConfiguration<UserNotificationRemoveEntity>
    {
        public void Configure(EntityTypeBuilder<UserNotificationRemoveEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }

    public class UserNotificationSeenConfiguration : IEntityTypeConfiguration<UserNotificationSeenEntity>
    {
        public void Configure(EntityTypeBuilder<UserNotificationSeenEntity> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}