using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models
{
    public class SocialNetworkUserModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid SocialNetworkId { set; get; }
        public Guid ProfileId { set; get; }

        public string SocialNetworkName { set; get; }
        public string SocialNetwokIcon { set; get; }
        public string SocialNetworkIconUrl { set; get; }
    }

    public class CreateSocialNetworkUserModel
    {
        public Guid SocialNetworkId { set; get; }
    }
}
