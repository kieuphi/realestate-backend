using Common.Shared.Entities;
using General.Domain.Enums;
using System;

namespace General.Domain.Entities
{
    public class PropertyEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string Slug { set; get; }
        public string PropertyNumber { set; get; }

        // =======================
        // PROPERTY SUMMARY
        // =======================
        public string TransactionTypeId { set; get; }
        public string PropertyTypeId { set; get; }
        public string CoverImage { set; get; }
        public string VideoLink { set; get; }
        public string VirtualVideoLink { set; get; }

        // =======================
        // PROPERTY ADDRESS
        // =======================
        public string Address { set; get; }

        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }

        public string Street { set; get; }
        public Guid ProjectId { set; get; }
        public string ProjectName { set; get; }

        public string PropertyAddressVi { set; get; }
        public string PropertyAddressEn { set; get; }

        // =======================
        // PROPERTY DESCRIPTION
        // =======================
        public string Title { set; get; }
        public string Descriptions { set; get; }

        // =======================
        // PROPERTY INFORMATIONS
        // =======================
        public decimal? LotSize { set; get; }
        public decimal? Price { set; get; }
        public decimal? USDPrice { set; get; }
        public string CurrencyId { set; get; }

        public string BedroomId { set; get; }
        public string BathroomId { set; get; }

        public decimal? FloorsNumber { set; get; }
        public decimal? TotalBuildingFloors { set; get; }

        public DateTime? YearCompleted { set; get; }

        public string Longitude { set; get; }
        public string Latitude { set; get; }

        // =======================
        // STATUS
        // =======================
        public Guid SupplierId { set; get; }
        public bool? IsShowSupplier { set; get; }

        // =======================
        // STATUS
        // =======================
        public Guid? TimeForPostId { set; get; }
        public PropertyApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }
        public DateTime? ExpiredDate { set; get; }
        public bool IsTemp { get; set; }
    }
}
