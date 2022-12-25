using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models
{
    public class ProjectViewCountModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string IPAddress { set; get; }
        public Guid ProjectId { set; get; }
        public int? ViewCount { set; get; }
    }

    public class CreateProjectViewCountModel
    {
        public Guid ProjectId { set; get; }
        public string IPAddress { set; get; }
    }
}
