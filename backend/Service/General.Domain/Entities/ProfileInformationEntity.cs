using Common.Shared.Entities;
using Common.Shared.Enums;
using General.Domain.Enums;
using System;

namespace General.Domain.Entities
{
    public class ProfileInformationEntity : AuditableEntity
    {
        public Guid Id { get; set; }
        
        // Common
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Address { get; set; }
        public string Descriptions { set; get; }
        public DateTime? BirthDay { get; set; }
        public GenderType GenderType { get; set; }
        public ActiveStatus Status { get; set; }

        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
        public string TitleDescriptionVi { set; get; }
        public string TitleDescriptionEn { set; get; }

        // Seller
        public string Agency { set; get; }

        // Supplier
        public string License { set; get; }
    }
}
