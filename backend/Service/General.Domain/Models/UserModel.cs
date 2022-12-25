using AutoMapper;
using Common.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using General.Domain.Common;
using System.Collections.Generic;
using General.Domain.Enums;

namespace General.Domain.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Descriptions { get; set; }
        public string Employee { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDelete { get; set; }
        public bool IsVerify { get; set; }
        [IgnoreMap]
        public string RoleId { get; set; }
        [IgnoreMap]
        public string Role { get; set; }
        public UserModel()
        {

        }
        public static UserModel ToUserModel(ApplicationUser applicationUser)
        {
            if (applicationUser == null) return default;

            return new UserModel
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                Descriptions = applicationUser.Descriptions,
                Employee = applicationUser.Employee,
                PhoneNumber = applicationUser.PhoneNumber,
                IsVerify = applicationUser.EmailConfirmed
            };
        }
    }
    
    public class CreateInternalUserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public Guid DepartmentId { get; set; }
        public string Notes { get; set; }
        public Guid AccountGroupId { get; set; }
        public GenderType? Gender { get; set; }
    }
    public class VerifyAccountModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //public string NewPassword { get; set; }

        //[DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        public string NewPassword { get; set; }

        //[DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { set; get; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //[DataType(DataType.Password)]
        //public string ConfirmPassword { get; set; }

        //public string ReturnUrl { get; set; }

        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber1 { set; get; }
        public string PhoneNumber2 { set; get; }
        public string PhoneNumber3 { set; get; }
        public string Descriptions { set; get; }
        public GenderType GenderType { get; set; }
        public RoleTypes Role { set; get; }

        public string TitleVi { set; get; }
        public string TitleEn { set; get; }
        public string TitleDescriptionVi { set; get; }
        public string TitleDescriptionEn { set; get; }

        public List<CreateSocialNetworkUserModel> SocialNetworks { set; get; }
    }
}
