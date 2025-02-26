using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.Services;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Data;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ForeignExchange.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasherService _passwordHasherService;

        public UserRepository(AppDbContext context, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> RegisterUserAsync(UserDTO userDto)
        {
            // Check if the username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return false; // User already exists
            }
            else if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return false; // User already exists
            }
            // Create new user
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _passwordHasherService.HashPassword(userDto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true; // Registration successful
        }

        public async Task<bool> UpdateUserEmailAsync(User user, string newEmail)
        {
            // Check if the new email already exists in the database
            var emailExists = await _context.Users.AnyAsync(u => u.Email == newEmail && u.Id != user.Id);

            if (emailExists)
            {
                return false; // Email is already taken
            }

            // Update the user's email
            user.Email = newEmail;

            // Attach the user to the context if it is not already tracked
            _context.Users.Update(user);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserUsernameAsync(User user, string newUsername)
        {
            // Check if the new username already exists in the database
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == newUsername && u.Id != user.Id);
            if (usernameExists)
            {
                return false;
            }

            // Update the user's username
            user.Username = newUsername;

            // Attach the user to the context if it is not already tracked
            _context.Users.Update(user);

            // Save changes to the database
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
