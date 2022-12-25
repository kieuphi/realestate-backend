using Common.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class PropertyViewCountEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public int ViewCount { get; set; }
        public int UserLoginViewCount { get; set; }
        public int UnLoginViewCount { get; set; }
    }
}
