using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;

namespace General.Domain.Models.PropertyElementModels
{
    public class PropertySellerModel : AuditableModel
    {
        public Guid Id { set; get; }
        public Guid PropertyId { set; get; }
        public Guid UserId { set; get; }

        public string Avatar { set; get; }
        public string AvatarUrl { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Email { set; get; }
        public string Agency { set; get; }
        public int? PropertyCount { set; get; }

        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
        public string TitleDescriptionVi { set; get; }
        public string TitleDescriptionEn { set; get; }

        public List<SocialNetworkUserModel> SocialNetworkUsers { set; get; }
    }

    public class CreatePropertySellerModel
    {
        public Guid UserId { set; get; }
    }
}
