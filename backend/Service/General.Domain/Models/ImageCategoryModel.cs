using System;

namespace General.Domain.Models
{
    public class ImageCategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
