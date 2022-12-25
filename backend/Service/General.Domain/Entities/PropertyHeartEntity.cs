using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities
{
    public class PropertyHeartEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid UserId { set; get; }
        public Guid PropertyId { set; get; }
    }
}
