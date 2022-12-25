using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Enums;
using General.Domain.Models.ProjectElementModels;

namespace General.Domain.Models
{
    public class ProjectModel : AuditableModel
    {
        // Short Info
        public Guid Id { set; get; }
        public string Slug { set; get; }
        public string PropertyTypeId { set; get; }
        public string PropertyTypeVi { set; get; }
        public string PropertyTypeEn { set; get; }

        public string ProjectVi { set; get; }
        public string ProjectEn { set; get; }
        public string CoverImage { set; get; }
        public string CoverImageUrl { set; get; }
        public string ProjectLogo { set; get; }
        public string ProjectLogoUrl { set; get; }
        public DateTime? StartDate { set; get; }
        public DateTime? EndDate { set; get; }
        public string Developer { set; get; }

        public string Video { set; get; }
        public string VirtualTour { set; get; }
        public string FloorPlans { set; get; }
        public string MapViewImage { set; get; }
        public string MapViewImageUrl { set; get; }

        public ProjectStatus Status { set; get; }
        public string StatusName { set; get; }

        // location
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }
        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public string WardName { set; get; }
        public string ProvinceNameEn { set; get; }
        public string DistrictNameEn { set; get; }
        public string WardNameEn { set; get; }
        public string Street { set; get; }

        // Descriptions
        public string Descriptions { set; get; }

        // Map View
        public string Longtitude { set; get; }
        public string Latitude { set; get; }

        public int? ViewCount { set; get; }

        // Approve Status
        public ProjectApproveStatus IsApprove { set; get; }
        public string ApproveStatusName { set; get; }
        public DateTime? ApproveDate { set; get; }

        // Project Element
        public List<ProjectFeatureModel> ProjectFeatures { set; get; }
        public List<ProjectImageModel> ProjectImages { set; get; }
        public List<ProjectSellerModel> ProjectSellers { set; get; }
    }

    public class CreateProjectModel
    {
        public string PropertyTypeId { set; get; }

        public string ProjectVi { set; get; }
        public string ProjectEn { set; get; }
        public string CoverImage { set; get; }
        public string ProjectLogo { set; get; }
        public DateTime? StartDate { set; get; }
        public DateTime? EndDate { set; get; }
        public string Developer { set; get; }

        public string Video { set; get; }
        public string VirtualTour { set; get; }
        public string FloorPlans { set; get; }
        public string MapViewImage { set; get; }

        // location
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }
        public string Street { set; get; }

        // Descriptions
        public string Descriptions { set; get; }

        // Map View
        public string Longtitude { set; get; }
        public string Latitude { set; get; }

        // Project Element
        public List<CreateProjectFeatureModel> ProjectFeatures { set; get; }
        public List<CreateProjectImageModel> ProjectImages { set; get; }
        public List<CreateProjectSellerModel> ProjectSellers { set; get; }
    }

    public class UpdateProjectModel : CreateProjectModel
    {
        public ProjectStatus Status { set; get; }
    }

    public class SearchProjectModel : PagingIndexModel
    {
        public string ProjectName { set; get; }
        public ProjectStatus? Status { set; get; }
        public string Location { set; get; }
        public string PropertyTypeId { set; get; }
        public SortingProjectModel SortingModel { set; get; }
    }

    public class SearchProjectForAdminModel : PagingIndexModel
    {
        public string ProjectName { set; get; }
        public ProjectApproveStatus? IsApprove { set; get; }
        public ProjectStatus? Status { set; get; }
        public string PropertyTypeId { set; get; }
        public SortingProjectModel SortingModel { set; get; }
    }

    public class SortingProjectModel
    {
        public bool? Latest { set; get; }
        public bool? Oldest { set; get; }
        public bool? MostView { set; get; }
        public bool? LeastView { set; get; }
    }

    public class PagingProjectModel : PagingIndexModel
    {

    }

    public class AdministrativeByProjectModel
    {
        public string Code { set; get; }
        public string Name { set; get; }
        public string NameWithType { set; get; }
    }

    // Suggest
    public class SuggestSearchProjectModel
    {
        public Guid ProjectId { set; get; }
        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
    }

    public class ImportProjectModel
    {
        public string PostNo { set; get; }

        public string ProjectVi { set; get; }
        public string ProjectEn { set; get; }
        public string PropertyTypeName { set; get; }
        public string StatusName { set; get; }
        public string Video { set; get; }
        public string VirtualTour { set; get; }
        public string FloorPlans { set; get; }
        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public string WardName { set; get; }
        public string Street { set; get; }

        public string ProjectFeatures { set; get; }
        public string Descriptions { set; get; }

        public string StartDate { set; get; }
        public string EndDate { set; get; }
        public string Developer { set; get; }
        public string Longtitude { set; get; }
        public string Latitude { set; get; }

        //public string MapViewImage { set; get; }
    }

    public class ImportProjectResultModel : ImportProjectModel
    {
        public string PropertyTypeId { set; get; }
        public ProjectStatus Status { set; get; }
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }
        // Project Element
        public List<string> ProjectFeatureIds { set; get; }
    }
}
