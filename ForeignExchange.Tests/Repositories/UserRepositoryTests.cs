using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Data;
using ForeignExchange.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ForeignExchange.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;
        private readonly Mock<IPasswordHasherService> _passwordHasherServiceMock;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByUsernameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetUserByUsernameAsync("nonexistentuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "test@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            var userDto = new User { Username = "newuser", Email = "newuser@example.com", PasswordHash = "password" };
            _passwordHasherServiceMock.Setup(m => m.HashPassword(It.IsAny<string>())).Returns("hashedpassword");

            // Act
            var result = await _repository.RegisterUserAsync(userDto);

            // Assert
            Assert.True(result);
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == "newuser");
            Assert.NotNull(dbUser);
            Assert.Equal("newuser@example.com", dbUser.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFalse_WhenUsernameExists()
        {
            // Arrange
            var existingUser = new User { Username = "existinguser", Email = "existing@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var userDto = new User { Username = "existinguser", Email = "newuser@example.com", PasswordHash = "password" };
            _passwordHasherServiceMock.Setup(m => m.HashPassword(It.IsAny<string>())).Returns("hashedpassword");

            // Act
            var result = await _repository.RegisterUserAsync(userDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserEmailAsync_ShouldReturnTrue_WhenEmailUpdatedSuccessfully()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "oldemail@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateUserEmailAsync(user, "newemail@example.com");

            // Assert
            Assert.True(result);
            Assert.Equal("newemail@example.com", user.Email);
        }

        [Fact]
        public async Task UpdateUserEmailAsync_ShouldReturnFalse_WhenEmailAlreadyExists()
        {
            // Arrange
            var existingUser = new User { Username = "existinguser", Email = "existing@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var user = new User { Username = "testuser", Email = "oldemail@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateUserEmailAsync(user, "existing@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserUsernameAsync_ShouldReturnTrue_WhenUsernameUpdatedSuccessfully()
        {
            // Arrange
            var user = new User { Username = "oldusername", Email = "user@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateUserUsernameAsync(user, "newusername");

            // Assert
            Assert.True(result);
            Assert.Equal("newusername", user.Username);
        }

        [Fact]
        public async Task UpdateUserUsernameAsync_ShouldReturnFalse_WhenUsernameAlreadyExists()
        {
            // Arrange
            var existingUser = new User { Username = "existinguser", Email = "existing@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var user = new User { Username = "oldusername", Email = "user@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.UpdateUserUsernameAsync(user, "existinguser");

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
