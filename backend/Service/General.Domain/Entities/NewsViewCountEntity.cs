using Common.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class NewsViewCountEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string IPAddress { set; get; }
        public Guid NewsId { set; get; }
        public int? ViewCount { set; get; }
    }
}
