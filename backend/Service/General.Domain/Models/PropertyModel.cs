using Common.Shared.Models;
using General.Domain.Enums;
using General.Domain.Models.PropertyElementModels;
using System;
using System.Collections.Generic;

namespace General.Domain.Models
{
    public class PropertyModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string Slug { set; get; }
        public string PropertyNumber { set; get; }

        // =======================
        // PROPERTY SUMMARY
        // =======================
        public string TransactionTypeId { set; get; }
        public string TransactionTypeVi { set; get; }
        public string TransactionTypeEn { set; get; }
        public string PropertyTypeId { set; get; }
        public string PropertyTypeVi { set; get; }
        public string PropertyTypeEn { set; get; }
        public string CoverImage { set; get; }
        public string CoverImageUrl { set; get; }

        public string VideoLink { set; get; }
        public string VirtualVideoLink { set; get; }

        // =======================
        // PROPERTY ADDRESS
        // =======================
        public string Address { set; get; }

        public string ProvinceCode { set; get; }
        public string ProvinceName { set; get; }
        public string ProvinceNameEn { set; get; }

        public string DistrictCode { set; get; }
        public string DistrictName { set; get; }
        public string DistrictNameEn { set; get; }

        public string WardCode { set; get; }
        public string WardName { set; get; }
        public string WardNameEn { set; get; }

        public string Street { set; get; }
        public Guid ProjectId { set; get; }
        public string ProjectName { set; get; }
        public string ProjectVi { set; get; }
        public string ProjectEn { set; get; }

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
        public decimal? LotSizeFeet { set; get; }
        public decimal? Price { set; get; }
        public decimal? USDPrice { set; get; }
        public string CurrencyId { set; get; }
        public string CurrencyName { set; get; }
        public string CurrencyNotation { set; get; }

        public string BedroomId { set; get; }
        public string BedroomVi { set; get; }
        public string BedroomEn { set; get; }
        public string BathroomId { set; get; }
        public string BathroomVi { set; get; }
        public string BathroomEn { set; get; }

        public decimal? FloorsNumber { set; get; }
        public decimal? TotalBuildingFloors { set; get; }

        public DateTime? YearCompleted { set; get; }

        public string Longitude { set; get; }
        public string Latitude { set; get; }

        // =======================
        // STATUS
        // =======================
        public Guid? TimeForPostId { set; get; }
        public decimal? TimeForPostValue { set; get; }
        public string TimeForPostName { set; get; }

        public decimal? TimeRemain { set; get; }

        public PropertyApproveStatus IsApprove { set; get; }
        public string ApproveStatusName { set; get; }
        public DateTime? ApproveDate { set; get; }
        public bool IsTemp { get; set; }
        public int ViewCount { get; set; }

        // =======================
        // SUPPLIER
        // =======================
        public Guid SupplierId { set; get; }
        public bool? IsShowSupplier { set; get; }
        public string SupplierAvatar { set; get; }
        public string SupplierFirstName { set; get; }
        public string SupplierLastName { set; get; }
        public string SupplierEmail { set; get; }
        public string SuppierPhoneNumber1 { set; get; }
        public string SuppierPhoneNumber2 { set; get; }
        public string SuppierTitleVi { set; get; }
        public string SuppierTitleEn { set; get; }
        public string SuppierTitleDescriptionVi { set; get; }
        public string SuppierTitleDescriptionEn { set; get; }

        // =======================
        // PROPERTY ELEMENTS
        // =======================
        public List<PropertyImageModel> PropertyImages { set; get; }
        public List<PropertyViewModel> PropertyViews { set; get; }
        public List<PropertyAmenitiesNearbyModel> PropertyAmenitiesNearbys { set; get; }
        public List<PropertySellerModel> PropertySellers { set; get; }
    }

    public class CreatePropertyModel
    {
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
        // SUPPLIER
        // =======================
        public Guid SupplierId { set; get; }
        public bool? IsShowSupplier { set; get; }

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

        public bool IsSubmit { get; set; }

        // =======================
        // PROPERTY ELEMENTS
        // =======================
        public List<CreatePropertyImageModel> PropertyImages { set; get; }
        public List<CreatePropertyViewModel> PropertyViews { set; get; }
        public List<CreatePropertyAmenitiesNearbyModel> PropertyAmenitiesNearbys { set; get; }
    }

    public class UpdatePropertyModel : CreatePropertyModel
    {
        // =======================
        // PROPERTY ELEMENTS
        // =======================
        public List<CreatePropertySellerModel> PropertySellers { set; get; }

        // =======================
        // MEETING NOTE
        // =======================
        public CreatePropertyMeetingNoteModel PropertyMeetingNote { set; get; }
    }

    public class SearchingPropertyModel : PagingIndexModel
    {
        // quick filter
        public string PropertyKeyWord { set; get; }
        public string PropertyTypeId { set; get; }
        public string TransactionTypeId { set; get; }
        public bool? IsManyCommercialProperty { set; get; }

        public string AdministrativeCode { set; get; }

        public decimal? MinPrice { set; get; }
        public decimal? MaxPrice { set; get; }

        public string BedroomId { set; get; }
        public string BathroomId { set; get; }

        //public decimal? FromLotSise { set; get; }
        //public decimal? ToLotSize { set; get; }

        public decimal? LandSize { set; get; }
        public DateTime? ListedSince { set; get; }

        // sorting
        public SortingPropertyModel SortingModel { set; get; }
    }

    public class SearchingPropertyForAdminModel : PagingIndexModel
    {
        // quick filter
        public string PropertyNumber { set; get; }
        public string PropertyTypeId { set; get; }
        public string TransactionTypeId { set; get; }

        public PropertyApproveStatus? IsApprove { set; get; }

        // sorting
        public SortingPropertyModel SortingModel { set; get; }
    }

    public class SortingPropertyModel
    {
        public bool? Newest { set; get; }
        public bool? Oldest { set; get; }
        public bool? LowestPrice { set; get; }
        public bool? HighestPrice { set; get; }
    }

    public class FilterPropertyByUserModel
    {
        public string TransactionTypeId { set; get; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<PropertyApproveStatus> ListStatus { get; set; }

    }

    public class ListPropertyModel : AuditableModel
    {
        public Guid Id { set; get; }
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
        public Guid? TimeForPostId { set; get; }
        public decimal? TimeForPostValue { set; get; }
        public string TimeForPostName { set; get; }
        public decimal? TimeRemain { set; get; }
        public DateTime? ApproveDate { set; get; }
        public DateTime? ExpiredDate { set; get; }
        public bool IsTemp { get; set; }
        public int ViewCount { get; set; }
    }

    public class BasicPropertyModel
    {
        public Guid Id { set; get; }
        public string Longitude { set; get; }
        public string Latitude { set; get; }
    }


    public class PagingPropertyModel : PagingIndexModel
    {

    }

    public class ApprovePropertyModel
    {
        public Guid PropertyId { set; get; }
        public Guid TimeForPostId { set; get; }
    }

    public class SuggestSearchPropertyRequestModel
    {
        public string Keyword { set; get; }
        public SuggestPropertyTypes? SuggestPropertyType { set; get; }
    }

    public class SuggestSearchPropertyModel
    {
        public List<SuggestPropertyAdministrativeModel> SuggestAdministrative { set; get; }
        public List<SuggestPropertyModel> SuggestProperty { set; get; }
    }

    public class SuggestPropertyAdministrativeModel
    {
        public string AdministrativeCode { set; get; }
        public string AdministrativeName { set; get; }
        public string AdministrativeNameEn { set; get; }
    }    
    
    public class SuggestPropertyModel
    {
        public Guid PropertyId { set; get; }
        public string PropertyNumber { set; get; }
        public string Address { set; get; }
    }

    public class PagingNearestPropertyModel : PagingIndexModel
    {
        public List<Guid> Ids { set; get; }
    }

    public class ImportPropertyModel
    {
        public string PostNo { set; get; }
        public string PropertyTypeName { set; get; }
        public string TransactionTypeName { set; get; }
        public string VideoLink { set; get; }
        public string VirtualVideoLink { set; get; }

        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public string WardName { set; get; }

        //public string ProvinceNameEn { set; get; }
        //public string DistrictNameEn { set; get; }
        //public string WardNameEn { set; get; }

        public string Street { set; get; }
        public string PropertyAddressVi { set; get; }
        public string PropertyAddressEn { set; get; }
        public string Longitude { set; get; }
        public string Latitude { set; get; }
        //public string ProjectName { set; get; }

        public string Title { set; get; }
        public string Descriptions { set; get; }
        public string PropertyViews { set; get; }
        public string LotSize { set; get; }
        public string Price { set; get; }
        public string CurrencyNotation { set; get; }

        public string BedroomName { set; get; }
        public string BathroomName { set; get; }

        public string FloorsNumber { set; get; }
        public string TotalBuildingFloors { set; get; }
        public string PropertyAmenitiesNearbys { set; get; }

        public string YearCompleted { set; get; }

        //public bool IsSubmit { get; set; }
        //public string PropertyImagePaths { set; get; }
        //public string CoverImage { set; get; }
    }

    public class ImportPropertyResultModel : ImportPropertyModel
    {
        public string TransactionTypeId { set; get; }
        public string PropertyTypeId { set; get; }
        public string ProvinceCode { set; get; }
        public string DistrictCode { set; get; }
        public string WardCode { set; get; }
        //public Guid ProjectId { set; get; }
        public string CurrencyId { set; get; }
        public string BedroomId { set; get; }
        public string BathroomId { set; get; }
        public string USDPrice { set; get; }
        public List<string> PropertyViewIds { set; get; }
        public List<string> PropertyAmenitiesNearbyIds { set; get; }
        //public List<Guid> PropertyImageIds { set; get; }
    }

    public class ExportTemplatePropertyModel
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }

    public class ExportPropertyMeetingNotesModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}
