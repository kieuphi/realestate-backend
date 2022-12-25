using Common.Shared.Entities;
using System;

namespace General.Domain.Entities
{
    public class AttachmentEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public long? FileSize { get; set; }
        public Guid AttachmentTypeId { get;set;}

        public virtual AttachmentTypeEntity AttachmentType { get; set; }

        public Guid ImageCategoryId { get; set; }

        public virtual ImageCategoryEntity ImageCategory { get; set; }

        //CHECKIN/ VISIT/ COMMENT
        public Guid? ReferenceId { get; set; }
    }
}
