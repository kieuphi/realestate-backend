using Common.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class UpdateProfileModel
    {
        public string AvatarFilePath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Address { get; set; }
        public string TitleVi { get; set; }
        public string TitleEn { get; set; }
        public string TitleDescriptionVi { get; set; }
        public string TitleDescriptionEn { get; set; }
        //public string Descriptions { set; get; }
        //public DateTime BirthDay { get; set; }
        //public GenderType GenderType { get; set; }
        //public ActiveStatus Status { get; set; }
        //public string RoleName { set; get; }

        // Seller
        //public string Agency { set; get; }

        // Supplier
        //public string License { set; get; }

        //public Guid? LevelDefinitionId { get; set; }
        //public Guid? TitleReferenceId { get; set; }
        //public Guid? DistributorId { get; set; }
    }
}
