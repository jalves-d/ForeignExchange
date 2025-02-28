using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Data;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForeignExchange.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> RegisterUserAsync(User newUser)
        {
            // Check if the username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == newUser.Username))
            {
                return false; // User already exists
            }
            else if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
            {
                return false; // User already exists
            }

            await _context.Users.AddAsync(newUser);
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
