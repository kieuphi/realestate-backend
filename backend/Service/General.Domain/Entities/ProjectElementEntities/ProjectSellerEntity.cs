using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities.ProjectElementEntities
{
    public class ProjectSellerEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
        public Guid UserId { set; get; }
    }
}
