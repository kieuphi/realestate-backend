using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;

namespace General.Domain.Entities
{
    public class SocialNetworkUserEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public Guid SocialNetworkId { set; get; }
        public Guid ProfileId { set; get; }
    }
}
