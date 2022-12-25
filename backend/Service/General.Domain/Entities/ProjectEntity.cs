using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;
using General.Domain.Enums;

namespace General.Domain.Entities
{
    public class ProjectEntity : AuditableEntity
    {
        // Short Info
        public Guid Id { set; get; }
        public string Slug { set; get; }
        public string PropertyTypeId { set; get; }
        public string ProjectVi { set; get; }
        public string ProjectEn { set; get; }
        public string ProjectLogo { set; get; }
        public string CoverImage { set; get; }
        public DateTime? StartDate { set; get; }
        public DateTime? EndDate { set; get; }
        public string Developer { set; get; }

        public string Video { set; get; }
        public string VirtualTour { set; get; }
        public string FloorPlans { set; get; }
        public string MapViewImage { set; get; }
        public DateTime? OpenForSaleDate { set; get; }

        // location
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }
        public string Street { set; get; }

        public ProjectStatus Status { set; get; }

        // Descriptions
        public string Descriptions { set; get; }

        // Map View
        public string Longtitude { set; get; }
        public string Latitude { set; get; }

        // Approve Status
        public ProjectApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }
    }
}
