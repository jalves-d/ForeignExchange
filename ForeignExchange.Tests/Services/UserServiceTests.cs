using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.Services;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ForeignExchange.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAcessor;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
            _httpContextAcessor = new Mock<IHttpContextAccessor>();
            _userService = new UserService(_userRepositoryMock.Object, _passwordHasherServiceMock.Object, _httpContextAcessor.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnTrue_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var userDto = new Domain.Entities.User { Username = "testuser", PasswordHash = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(userDto)).ReturnsAsync(true);

            // Act
            var user = new UserDTO { Username = "testuser", Password = "password123" };
            var result = true;
            try{ await _userService.RegisterUserAsync(user); }
            catch (Exception) { result = false; };

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFalse_WhenUserRegistrationFails()
        {
            // Arrange
            var userDto = new Domain.Entities.User { Username = "testuser", PasswordHash = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(userDto));

            var user = new UserDTO { Username = "testuser", Password = "password123" };
            // Act
            var result = true;
            try { await _userService.RegisterUserAsync(user); }
            catch (Exception) { result = false; };

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserRepositoryThrowsException()
        {
            // Arrange
            var user = new Domain.Entities.User { Username = "testuser", PasswordHash = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(user)).ThrowsAsync(new System.Exception("Database error"));

            // Act & Assert
            var userDto = new UserDTO { Username = "testuser", Password = "password123" };
            await Assert.ThrowsAsync<System.Exception>(() => _userService.RegisterUserAsync(userDto));
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(user), Times.Once);
        }
    }

}
