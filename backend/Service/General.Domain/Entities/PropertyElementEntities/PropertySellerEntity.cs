using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.PropertyElementEntities
{
    public class PropertySellerEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public Guid UserId { set; get; }
    }
}
