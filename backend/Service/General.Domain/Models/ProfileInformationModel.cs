using Common.Shared.Enums;
using Common.Shared.Models;
using General.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class ProfileInformationModel: AuditableModel
    {
        public Guid Id { set; get; }
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string AvatarUrl { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Address { get; set; }
        public string Descriptions { set; get; }
        public DateTime BirthDay { get; set; }
        public GenderType GenderType { get; set; }
        public ActiveStatus Status { get; set; }
        public string RoleName { set; get; }

        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
        public string TitleDescriptionVi { set; get; }
        public string TitleDescriptionEn { set; get; }

        // Seller
        public string Agency { set; get; }

        // Supplier
        public string License { set; get; }

        // User
        public string UserName { get; set; }
        public string Email { get; set; }

        public int? PropertyCount { set; get; }

        public List<SocialNetworkUserModel> SocialNetworks { set; get; }
    }

    public class UpdateProfileInformationModel
    {
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string AvatarUrl { set; get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Address { get; set; }
        public string Descriptions { set; get; }
        public DateTime BirthDay { get; set; }
        public GenderType GenderType { get; set; }

        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
        public string TitleDescriptionVi { set; get; }
        public string TitleDescriptionEn { set; get; }

        // Seller
        public string Agency { set; get; }

        // Supplier
        public string License { set; get; }

        // User
        public string UserName { get; set; }
        public string Email { get; set; }

        public List<CreateSocialNetworkUserModel> SocialNetworks { set; get; }
    }

    public class PagingProfileInformationModel : PagingIndexModel
    {

    }
}
