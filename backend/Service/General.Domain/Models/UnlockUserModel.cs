using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class UnlockUserModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
