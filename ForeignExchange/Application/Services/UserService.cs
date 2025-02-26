using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Infrastructure.Interfaces;

namespace ForeignExchange.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(UserDTO userDto)
        {
            return await _userRepository.RegisterUserAsync(userDto);
        }
    }
}
