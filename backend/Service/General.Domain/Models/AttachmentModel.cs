using AutoMapper;
using General.Domain.Enumerations;
using System;
using System.Collections.Generic;

namespace General.Domain.Models
{
    public class AttachmentModel
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public long? FileSize { get; set; }
        public DateTime Created { get; set; }
        public Guid AttachmentTypeId { get; set; }
        public AttachmentTypeModel AttachmentType { get; set; }
        public Guid ServiceTypeId { get; set; }

        public Guid? ReferenceId { get; set; }

        [IgnoreMap]
        public string AttachmentTypeName
        {
            get
            {
                if (AttachmentType != null)
                {
                    return AttachmentType.Name;
                }

                return string.Empty;
            }
            set { }
        }
    }

    public class AttachmentCollectionFilterModel
    {
        public string AttachmentType { get; set; }
        public string ServiceType { get; set; }
        public List<Guid?> ReferenceIds { get; set; }
    }
}
