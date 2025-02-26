using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Services;
using ForeignExchange.Infrastructure.Interfaces;
using Moq;

namespace ForeignExchange.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnTrue_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var userDto = new UserDTO { Username = "testuser", Password = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(userDto)).ReturnsAsync(true);

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFalse_WhenUserRegistrationFails()
        {
            // Arrange
            var userDto = new UserDTO { Username = "testuser", Password = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(userDto)).ReturnsAsync(false);

            // Act
            var result = await _userService.RegisterUserAsync(userDto);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserRepositoryThrowsException()
        {
            // Arrange
            var userDto = new UserDTO { Username = "testuser", Password = "password123" };
            _userRepositoryMock.Setup(repo => repo.RegisterUserAsync(userDto)).ThrowsAsync(new System.Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _userService.RegisterUserAsync(userDto));
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(userDto), Times.Once);
        }
    }

}
