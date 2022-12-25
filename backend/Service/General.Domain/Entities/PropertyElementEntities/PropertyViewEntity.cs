using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.PropertyElementEntities
{
    public class PropertyViewEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public string ViewId { set; get; }
    }
}
