using FluentAssertions;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Exceptions;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.Services;
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
        public async Task RegisterUserAsync_ShouldSaveUserToDatabase_WhenSuccessful()
        {
            // Arrange
            var user = new User { Username = "newuser", Email = "newuser@example.com", PasswordHash = "password" };

            // Act
            await _repository.RegisterUserAsync(user);

            // Assert
            var dbUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
            Assert.NotNull(dbUser);
            Assert.Equal(user.Email, dbUser.Email);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameExists()
        {
            // Arrange
            var existingUser = new User { Username = "existinguser", Email = "existing@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var userDto = new User { Username = "existinguser", Email = "newuser@example.com", PasswordHash = "password" };

            // Act
            Func<Task> act = async () => await _repository.RegisterUserAsync(userDto);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Invalid credentials! Email or Username in use.");
        }

        [Fact]
        public async Task UpdateUserEmailAsync_ShouldNotThrowException_WhenEmailUpdatedSuccessfully()
        {
            // Arrange
            var user = new User { Username = "testuser", Email = "oldemail@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.UpdateUserEmailAsync(user, "newemail@example.com");

            // Assert
            await act.Should().NotThrowAsync<Exception>();
            Assert.Equal("newemail@example.com", user.Email);
        }

        [Fact]
        public async Task UpdateUserUsernameAsync_ShouldNotThrowException_WhenUsernameUpdatedSuccessfully()
        {
            // Arrange
            var user = new User { Username = "oldusername", Email = "user@example.com", PasswordHash = "hashedpassword" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.UpdateUserUsernameAsync(user, "newusername");

            // Assert
            await act.Should().NotThrowAsync<Exception>();
            Assert.Equal("newusername", user.Username);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
