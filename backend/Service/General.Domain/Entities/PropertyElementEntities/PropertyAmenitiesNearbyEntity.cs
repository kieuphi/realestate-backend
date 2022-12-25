using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.PropertyElementEntities
{
    public class PropertyAmenitiesNearbyEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public string AmenitiesNearbyId { set; get; }
    }
}
