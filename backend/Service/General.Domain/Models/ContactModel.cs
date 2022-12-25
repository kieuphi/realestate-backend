using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class ContactModel : AuditableModel
    {
        public Guid Id { set; get; }
        public string Subject { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Message { set; get; }
        public ContactType ContactType { get; set; }
    }

    public class CreateContactModel
    {
        public string Subject { set; get; }
        public string Name { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Message { set; get; }
        public string PropertyId { get; set; }
        public string ProjectId { get; set; }
        public ContactType ContactType { get; set; }
    }

    public class PagingContactModel : PagingIndexModel
    {

    }
}
