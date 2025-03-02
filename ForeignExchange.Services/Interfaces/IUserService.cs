using System.Threading.Tasks;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Domain.Entities;

namespace ForeignExchange.Application.Interfaces
{
    public interface IUserService
    {
        Task RegisterUserAsync(UserDTO userDto);
        Task DeleteUserAsync(string user);
        Task DeleteUserAsync();
        Task UpdateUsernameAsync(string newUsername);
        Task UpdateEmailAsync(string newEmail);
    }
}
