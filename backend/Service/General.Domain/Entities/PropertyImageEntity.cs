using Common.Shared.Entities;
using Common.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class PropertyImageEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagesPath { get; set; }
        public Guid PropertyId { set; get; }
        public string Notes { get; set; }
    }
}
