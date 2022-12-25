using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace General.Domain.Models
{
    public class RefreshTokenModel
    {
        [JsonPropertyName("refreshToken")]
        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
