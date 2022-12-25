using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities
{
    public class TimeForPostEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public decimal? Value { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
    }
}
