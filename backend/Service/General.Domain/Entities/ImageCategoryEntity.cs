using Common.Shared.Entities;
using General.Domain.Entities;
using System;
using System.Collections.Generic;

namespace General.Domain.Entities
{
    public class ImageCategoryEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<AttachmentEntity> Attachments { get; set; }
    }
}
