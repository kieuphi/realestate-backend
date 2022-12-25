using Common.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Domain.Entities
{
    public class UserSavedSearchEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }
        public string Type { get; set; }
    }
}
