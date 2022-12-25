using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class PropertyHeartModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid UserId { set; get; }
        public Guid PropertyId { set; get; }
    }

    public class CreatePropertyHeartModel
    {
        public Guid UserId { set; get; }
        public Guid PropertyId { set; get; }
    }

    public class ListPropertyHeartModel
    {
        public Guid UserId { set; get; }
        public Guid PropertyId { set; get; }
        public string Slug { set; get; }

        public string Title { set; get; }
        public string PropertyNumber { set; get; }

        public string CoverImage { set; get; }
        public string CoverImageUrl { set; get; }

        public string PropertyAddressVi { set; get; }
        public string PropertyAddressEn { set; get; }
        public decimal? Price { set; get; }
        public decimal? USDPrice { set; get; }

        public string CurrencyId { set; get; }
        public string CurrencyName { set; get; }
        public string CurrencyNotation { set; get; }

        public string TransactionTypeId { set; get; }
        public string TransactionTypeVi { set; get; }
        public string TransactionTypeEn { set; get; }

        public string BedroomId { set; get; }
        public string BedroomVi { set; get; }
        public string BedroomEn { set; get; }
        public string BathroomId { set; get; }
        public string BathroomVi { set; get; }
        public string BathroomEn { set; get; }

        public string PropertyTypeId { set; get; }
        public string PropertyTypeVi { set; get; }
        public string PropertyTypeEn { set; get; }

        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }

        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public string WardName { set; get; }

        public string ProvinceNameEn { set; get; }
        public string DistrictNameEn { set; get; }
        public string WardNameEn { set; get; }

        public string CoordinatesProvince { set; get; }
        public string CoordinatesDistrict { set; get; }
        public string CoordinatesWard { set; get; }

        public string Location { set; get; }
        public string LocationEn { set; get; }
        public decimal? LotSize { set; get; }
        public decimal? LotSizeFeet { set; get; }

        public string Longitude { set; get; }
        public string Latitude { set; get; }

        public PropertyApproveStatus IsApprove { set; get; }
        public string ApproveStatusName { set; get; }

        public DateTime? ApproveDate { set; get; }
        public DateTime CreateTime { get; set; }
    }
    
    public class PagingPropertyHeartModel : PagingIndexModel
    {
        public Guid UserId { set; get; }

        // sorting
        public SortingFavouriteModel SortingModel { set; get; }
    }

    public class SortingFavouriteModel
    {
        public bool? Newest { set; get; }
        public bool? Oldest { set; get; }
        public bool? LowestPrice { set; get; }
        public bool? HighestPrice { set; get; }
        public bool? OldFavourite { set; get; }
        public bool? NewFavourite { set; get; }
    }
}
