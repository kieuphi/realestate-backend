using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities
{
    public class PropertyNearestEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid UserId { set; get; }
        public Guid PropertyId { set; get; }
    }
}
