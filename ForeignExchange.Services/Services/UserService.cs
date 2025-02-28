using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Interfaces;

namespace ForeignExchange.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<bool> RegisterUserAsync(UserDTO userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _passwordHasherService.HashPassword(userDto.Password)
            };
            return await _userRepository.RegisterUserAsync(user);
        }
    }
}
