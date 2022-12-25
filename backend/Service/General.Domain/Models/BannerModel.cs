using System;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class BannerModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string BannerName { set; get; }
        public BannerTypes? BannerType { set; get; }
        public string ImageUrl { set; get; }
        public string ImagePathUrl { set; get; }
        public string Descriptions { set; get; }
        public int? BannerOrder { set; get; }
        public string BannerTypeName { set; get; }
    }

    public class CreateBannerModel
    {
        public string BannerName { set; get; }
        public BannerTypes? BannerType { set; get; }
        public string ImageUrl { set; get; }
        public string ImagePathUrl { set; get; }
        public string Descriptions { set; get; }
        public int? BannerOrder { set; get; }
    }

    public class PagingBannerModel : PagingIndexModel
    {
    }
}
