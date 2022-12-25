using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class RegisterUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

        [Required]
        
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
    }
}
