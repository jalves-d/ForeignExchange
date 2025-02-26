using ForeignExchange.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ForeignExchange.Application.DTOs
{
    public class UserDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
