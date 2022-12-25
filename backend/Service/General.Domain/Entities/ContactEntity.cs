using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Entities;
using General.Domain.Enums;

namespace General.Domain.Entities
{
    public class ContactEntity : AuditableEntity
    {
        public Guid Id { set; get; }
        public string Subject { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Message { set; get; }
        public string PropertyId { get; set; }
        public ContactType ContactType { get; set; }
    }
}
