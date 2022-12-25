using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.ProjectElementEntities
{
    public class ProjectFeatureEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
        public string ProjectFeatureId { set; get; }
    }
}
