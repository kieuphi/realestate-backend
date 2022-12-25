using System;

namespace General.Domain.Models
{
    public class AttachmentTypeModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
