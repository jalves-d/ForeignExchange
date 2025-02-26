using ForeignExchange.Application.DTOs;
using ForeignExchange.Domain.Entities;
using System.Threading.Tasks;

namespace ForeignExchange.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> RegisterUserAsync(UserDTO userDto);
    }
}
