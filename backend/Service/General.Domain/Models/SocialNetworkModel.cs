using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models
{
    public class SocialNetworkModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string AppName { set; get; }
        public string ICon { set; get; }
        public string Descriptions { set; get; }
        public string IConUrl { set; get; }
        public bool? IsShowFooter { set; get; }
    }

    public class CreateSocialNetworkModel
    {
        public string AppName { set; get; }
        public string ICon { set; get; }
        public string Descriptions { set; get; }
        public bool? IsShowFooter { set; get; }
    }

    public class PagingSocialNetworkModel : PagingIndexModel
    {

    }
}
