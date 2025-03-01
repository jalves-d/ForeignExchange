using ForeignExchange.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ForeignExchange.Application.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
