using Microsoft.AspNetCore.Identity;
using System;
using General.Domain.Entities;

namespace General.Domain.Common
{
    public class ApplicationUser : IdentityUser
    {
        public string Employee { get; set; }
        public string Descriptions { get; set; }
        public bool IsDelete { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModified { get; set; }
    }
}
