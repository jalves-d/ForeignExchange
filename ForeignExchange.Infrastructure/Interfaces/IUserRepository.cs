using ForeignExchange.Domain.Entities;
using System.Threading.Tasks;

namespace ForeignExchange.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task RegisterUserAsync(User userDto);
        Task DeleteUserAsync(User user);
        Task UpdateUserUsernameAsync(User user, string newUsername);
        Task UpdateUserEmailAsync(User user, string newEmail);
    }
}
