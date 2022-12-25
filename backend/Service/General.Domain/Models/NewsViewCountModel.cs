using Common.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Models
{
    public class NewsViewCountModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string IPAddress { set; get; }
        public Guid NewsId { set; get; }
        public int? ViewCount { set; get; }
    }

    public class CreateNewsViewCountModel
    {
        public Guid ProjectId { set; get; }
        public string IPAddress { set; get; }
    }
}
