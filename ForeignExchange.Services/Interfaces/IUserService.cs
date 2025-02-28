using System.Threading.Tasks;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Domain.Entities;

namespace ForeignExchange.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserDTO userDto);
    }
}
