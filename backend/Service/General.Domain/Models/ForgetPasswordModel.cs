using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class ForgetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
