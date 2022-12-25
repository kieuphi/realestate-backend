using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class UserSavedSearchModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }
        public string Type { get; set; }
        public DateTime CreateTime { get; set; }
        public int NumberOfDaysAgo { get; set; }
    }

    public class CreateUserSavedSearchModel
    {
        public string Name { get; set; }
        public string Keyword { get; set; }
        public string Type { get; set; }
    }

    public class SortingUserSavedSearchModel
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool? Newest { set; get; }
        public bool? Oldest { set; get; }
        public bool? NameOrder { set; get; }
        public bool? NameOrderDescending { set; get; }
    }

   
}
