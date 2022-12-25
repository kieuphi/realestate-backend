using Common.Shared.Entities;
using General.Domain.Common;
using General.Domain.Enums;
using System;
using System.Collections.Generic;

namespace General.Domain.Entities
{
    public class NewsCategoryEntity: AuditableEntity
    {
        public Guid Id { get; set; }
        public string CategoryNameVi { get; set; }
        public string CategoryNameEn { set; get; }
        public NewsApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }

        public virtual ICollection<NewsEntity> News { get; set; }
    }
}
