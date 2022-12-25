using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models
{
    public class TimeForPostModel : AuditableModel
    {
        public Guid Id { set; get; }
        public decimal? Value { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
    }

    public class CreateTimeForPostModel
    {
        public decimal? Value { set; get; }
        public string DisplayName { set; get; }
        public string Description { set; get; }
    }

    public class PagingTimeForPostModel : PagingIndexModel
    {

    }
}
