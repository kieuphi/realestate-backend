using AutoMapper;
using System;
using General.Domain.Models;

namespace General.Application.Common.Results
{
    public class UserResult
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool EmailConfirmed { set; get; }

        [IgnoreMap]
        public string RoleName { get; set; }
        public ProfileInformationModel PersonalInformation { get; set; }

    }
}
