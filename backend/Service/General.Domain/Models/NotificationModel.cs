using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TitleVi { get; set; }
        public string Content { get; set; }
        public string ContentVi { get; set; }
        public string Link { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedTime { get; set; }
        public int NumberOfDaysAgo { get; set; }
        public string PostedBy { get; set; }
    }

    public class NotificationUserModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TitleVi { get; set; }
        public string Content { get; set; }
        public string ContentVi { get; set; }
        public string Link { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedTime { get; set; }
        public int NumberOfDaysAgo { get; set; }
        public string PostedBy { get; set; }
        public bool IsSeen { get; set; }
    }

    public class CreateNotificationModel
    {
        public string Title { get; set; }
        public string TitleVi { get; set; }
        public string Content { get; set; }
        public string ContentVi { get; set; }
        public string Link { get; set; }
        public bool IsPosted { get; set; }
    }

    public class UpdateNotificationModel
    {
        public string Title { get; set; }
        public string TitleVi { get; set; }
        public string Content { get; set; }
        public string ContentVi { get; set; }
        public string Link { get; set; }
    }

    public class SortingNotificationModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool? Newest { set; get; }
        public bool? Oldest { set; get; }
        public bool? NameOrder { set; get; }
        public bool? NameOrderDescending { set; get; }
    }
    public class FilterNotificationModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Keyword { get; set; }
        public bool? IsPosted { get; set; }
    }

    
}
