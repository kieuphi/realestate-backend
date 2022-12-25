using System;
using System.Collections.Generic;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class NewsModel : AuditableModel
    {
        public Guid Id { get; set; }
        public string Slug { set; get; }
        public string TitleVi { get; set; }
        public string TitleEn { get; set; }

        public string Keyword { get; set; }

        public string ContentVi { get; set; }
        public string ContentEn { get; set; }

        public int StatusId { get; set; }

        public string DescriptionsVi { get; set; }
        public string DescriptionsEn { get; set; }

        public Guid CategoryId { get; set; }

        public string ImageUrl { get; set; }
        public string ImagePathUrl { set; get; }

        public string CategoryName { get; set; }
        public bool? Featured { get; set; }
        public bool? IsHotNews { set; get; }
        public bool? IsWellRead { set; get; }

        public int? ViewCount { set; get; }

        public NewsApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }
        public NewsPositionTypes Position { set; get; }

        public virtual NewsCategoryModel Category { get; set; }
    }

    public class CreateNewsModel
    {
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
    }

    public class ListNewsModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string Slug { set; get; }
        public string ImageUrl { set; get; }
        public string ImagePathUrl { set; get; }

        public string DescriptionsVi { get; set; }
        public string DescriptionsEn { get; set; }

        public string TitleVi { get; set; }
        public string TitleEn { set; get; }

        public Guid CategoryId { get; set; }

        public bool? Featured { get; set; }
        public bool? IsHotNews { set; get; }
        public bool? IsWellRead { set; get; }

        public int? ViewCount { set; get; }

        public NewsApproveStatus IsApprove { set; get; }
        public DateTime? ApproveDate { set; get; }
        public string IsApproveName { set; get; }

        public virtual NewsCategoryModel Category { get; set; }
    }

    public class PagingNewsModel : PagingIndexModel
    {

    }

    public class SearchingNewsModel : PagingIndexModel
    {
        public string Title { set; get; }
    }

    public class SearchingNewsForAdminModel : SearchingNewsModel
    {
        public NewsApproveStatus? IsApprove { set; get; }
        public Guid? CategoryId { set; get; }
        public bool? IsHotNew { set; get; }
    }
    
    public class ListNewsGroupByCategoryModel : AuditableModel
    {
        public Guid Id { get; set; }
        public string CategoryNameVi { get; set; }
        public string CategoryNameEn { get; set; }

        public List<ListNewsModel> News { set; get; }
    }
}
