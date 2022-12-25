using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.ProjectElementEntities
{
    public class ProjectImageEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImagesPath { get; set; }
        public Guid ProjectId { set; get; }
    }
}
