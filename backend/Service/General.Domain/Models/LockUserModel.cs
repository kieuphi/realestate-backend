using System.ComponentModel.DataAnnotations;

namespace General.Domain.Models
{
    public class LockUserModel
    {
        [Required]
        public string UserId { get; set; }
    }
}
