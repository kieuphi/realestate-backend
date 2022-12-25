using Common.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class ProjectViewCountEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string IPAddress { set; get; }
        public Guid ProjectId { set; get; }
        public int? ViewCount { set; get; }
    }
}
