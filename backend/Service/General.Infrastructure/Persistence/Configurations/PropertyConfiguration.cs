using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Persistence.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<PropertyEntity>
    {
        public void Configure(EntityTypeBuilder<PropertyEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PropertyTypeId).IsRequired();
            //builder.Property(x => x.PropertyCost).IsRequired();
        }
    }
}