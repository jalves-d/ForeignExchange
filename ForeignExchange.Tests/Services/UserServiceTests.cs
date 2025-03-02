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
        public async Task RegisterUserAsync_ShouldHaveUserRegistered_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()))
                .Returns(Task.CompletedTask);

            var userDto = new UserDTO { Username = "testuser", Password = "password123" };

            // Act
            await _userService.RegisterUserAsync(userDto);

            // Assert
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
        }


        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserRegistrationFails()
        {
            // Arrange
            var userDto = new Domain.Entities.User { Username = "testuser", PasswordHash = "password123" };

            _userRepositoryMock
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()))
                .ThrowsAsync(new Exception("Registration failed"));

            var user = new UserDTO { Username = "testuser", Password = "password123" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.RegisterUserAsync(user));

            // Verifica se o repositório foi chamado corretamente
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserRepositoryThrowsException()
        {
            // Arrange
            _userRepositoryMock
                .Setup(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()))
                .ThrowsAsync(new System.Exception("Register User process failed due to:"));

            var userDto = new UserDTO { Username = "testuser", Password = "password123" };

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _userService.RegisterUserAsync(userDto));

            // Verifica se o repositório foi chamado corretamente
            _userRepositoryMock.Verify(repo => repo.RegisterUserAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
        }
    }

}
