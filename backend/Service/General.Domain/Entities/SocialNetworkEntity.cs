using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities
{
    public class SocialNetworkEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string AppName { set; get; }
        public string ICon { set; get; }
        public string Descriptions { set; get; }
        public bool? IsShowFooter { set; get; }
    }
}
