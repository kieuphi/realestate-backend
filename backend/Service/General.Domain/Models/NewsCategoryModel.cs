using System;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class NewsCategoryModel : AuditableModel
    {
        public Guid Id { get; set; }
        public string CategoryNameVi { get; set; }
        public string CategoryNameEn { get; set; }

        public NewsApproveStatus? IsApprove { set; get; }
    }

    public class CreateNewsCategoryModel
    {
        public string CategoryNameVi { get; set; }
        public string CategoryNameEn { get; set; }
    }

    public class ListNewsCategoryModel : AuditableModel
    {
        public Guid Id { get; set; }
        public string CategoryNameVi { get; set; }
        public string CategoryNameEn { get; set; }

        public NewsApproveStatus IsApprove { set; get; }
        public string IsApproveName { set; get; }
        public DateTime? ApproveDate { set; get; }
    }

    public class PagingNewsCategoryModel : PagingIndexModel
    {

    }

    public class SearchingNewsCategoryModel : PagingIndexModel
    {
        public string Title { set; get; }
        public NewsApproveStatus? IsApprove { set; get; }
    }
}
