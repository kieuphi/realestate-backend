using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.ProjectElementModels
{
    public class ProjectFeatureModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
        public string ProjectFeatureId { set; get; }

        public string ProjectFeatureVi { set; get; }
        public string ProjectFeatureEn { set; get; }
        public string Descriptions { set; get; }
    }

    public class CreateProjectFeatureModel
    {
        public string ProjectFeatureId { set; get; }
    }
}
