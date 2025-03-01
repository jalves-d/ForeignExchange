using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ForeignExchange.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterUserAsync(UserDTO userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _passwordHasherService.HashPassword(userDto.Password)
            };
            await _userRepository.RegisterUserAsync(user);
        }

        public async Task DeleteUserAsync(string user)
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
                
            await _userRepository.DeleteUserAsync(userToDelete);
        }
        private string? GetLoggedUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value;
        }

        public async Task DeleteUserAsync()
        {
            try { 
                var user = await _userRepository.GetUserByUsernameAsync(GetLoggedUsername());

                await _userRepository.DeleteUserAsync(user);
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

    }
}
