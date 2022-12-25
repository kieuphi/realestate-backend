using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Persistence.Configurations
{
    public class UserSavedSearchConfiguration : IEntityTypeConfiguration<UserSavedSearchEntity>
    {
        public void Configure(EntityTypeBuilder<UserSavedSearchEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}