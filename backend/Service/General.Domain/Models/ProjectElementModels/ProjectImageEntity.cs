using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.ProjectElementModels
{
    public class ProjectImageModel : AuditableModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagesPath { get; set; }
        public string ImagesPathUrl { get; set; }
        public Guid ProjectId { set; get; }
    }

    public class CreateProjectImageModel
    {
        public string Name { get; set; }
        public string ImagesPath { get; set; }
    }
}
