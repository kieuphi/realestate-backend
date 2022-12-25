using General.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Infrastructure.Persistence.Configurations
{
    public class PropertyNumberCounterConfiguration : IEntityTypeConfiguration<PropertyNumberCounterEntity>
    {
        public void Configure(EntityTypeBuilder<PropertyNumberCounterEntity> builder)
        {
            //builder.HasNoKey();
        }
    }
}
