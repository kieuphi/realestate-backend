using Common.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class CreateUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { set; get; }

        [Required]
        public string PhoneNumber { get; set; }
        public Guid? RoleDefinitionId { get; set; }

        public string LastName { get; set; }
        public string SirName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime BirthDay { get; set; }
        public ActiveStatus Status { get; set; }
        public GenderType GenderType { get; set; }
        public Guid? LevelDefinitionId { get; set; }
        public Guid? TitleReferenceId { get; set; }
        public Guid? DistributorId { get; set; }
    }
    
}
