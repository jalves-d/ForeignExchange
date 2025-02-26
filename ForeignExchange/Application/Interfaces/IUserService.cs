using System.Threading.Tasks;
using ForeignExchange.Application.DTOs;

namespace ForeignExchange.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserDTO userDto);
    }
}
