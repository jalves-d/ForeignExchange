using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<bool> DeleteUserAsync(string user)
        {
            User userToDelete = null;
            if (!user.IsNullOrEmpty())
            {
                userToDelete = await _userRepository.GetUserByUsernameAsync(user);
                if (userToDelete != null)
                {
                    userToDelete = await _userRepository.GetUserByEmailAsync(user);
                }
                else
                    throw new Exception();
            }
            else
                throw new Exception();
                
            return await _userRepository.DeleteUserAsync(userToDelete);
        }
    }
}
