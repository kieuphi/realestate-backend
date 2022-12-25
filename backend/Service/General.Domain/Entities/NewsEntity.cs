using Common.Shared.Entities;
using General.Domain.Common;
using General.Domain.Enums;
using System;

namespace General.Domain.Entities
{
    public class NewsEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Slug { set; get; }
        public string TitleVi { get; set; }
        public string TitleEn { set; get; }

        public string Keyword { get; set; }

        public string ContentVi { get; set; }
        public string ContentEn { set; get; }

        public string DescriptionsVi { get; set; }
        public string DescriptionsEn { get; set; }

        public Guid CategoryId { get; set; }

        public string ImageUrl { get; set; }
        public bool? Featured { get; set; }
        public bool? IsHotNews { set; get; }
        public bool? IsWellRead { set; get; }
        public NewsPositionTypes? Position { set; get; }

        public NewsApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }

        public virtual NewsCategoryEntity Category { get; set; }
    }
}
