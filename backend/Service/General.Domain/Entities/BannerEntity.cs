using System;
using Common.Shared.Entities;
using General.Domain.Enums;

namespace General.Domain.Entities
{
    public class BannerEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string BannerName { set; get; }
        public BannerTypes? BannerType { set; get; }
        public string ImageUrl { set; get; }
        public string Descriptions { set; get; }
        public int? BannerOrder { set; get; }
    }
}
