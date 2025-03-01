using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Exceptions;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.Services;

namespace ForeignExchange.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherService> _passwordHasherServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.SetupGet(c => c["JwtSettings:SecretKey"]).Returns("your_secret_keyawesomekeynewone2025");
            _configurationMock.SetupGet(c => c["JwtSettings:Issuer"]).Returns("your_issuer");
            _configurationMock.SetupGet(c => c["JwtSettings:Audience"]).Returns("your_audience");

            _authService = new AuthService(
                _userRepositoryMock.Object,
                _passwordHasherServiceMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDTO = new LoginDTO { Login = "testUser", Password = "password123" };
            var user = new User { Id = Guid.NewGuid(), Username = "testUser", PasswordHash = "hashedPassword" };

            _userRepositoryMock.Setup(u => u.GetUserByUsernameAsync(loginDTO.Login)).ReturnsAsync(user);
            _passwordHasherServiceMock.Setup(p => p.VerifyPassword(loginDTO.Password, user.PasswordHash)).Returns(true);

            // Act
            var token = await _authService.AuthenticateAsync(loginDTO);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenUsernameNotFound()
        {
            // Arrange
            var loginDTO = new LoginDTO { Login = "nonExistentUser", Password = "password123" };

            _userRepositoryMock.Setup(u => u.GetUserByUsernameAsync(loginDTO.Login)).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(loginDTO);

            // Assert
            await act.Should().ThrowAsync<InvalidCredentialCustomException>()
                .WithMessage("Invalid credentials, username not found!");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenEmailNotFound()
        {
            // Arrange
            var loginDTO = new LoginDTO { Login = "nonExistentEmail@test.com", Password = "password123" };

            _userRepositoryMock.Setup(u => u.GetUserByEmailAsync(loginDTO.Login)).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(loginDTO);

            // Assert
            await act.Should().ThrowAsync<InvalidCredentialCustomException>()
                .WithMessage("Invalid credentials, email not found!");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginDTO = new LoginDTO { Login = "testUser", Password = "wrongPassword" };
            var user = new User { Id = Guid.NewGuid(), Username = "testUser", PasswordHash = "hashedPassword" };

            _userRepositoryMock.Setup(u => u.GetUserByUsernameAsync(loginDTO.Login)).ReturnsAsync(user);
            _passwordHasherServiceMock.Setup(p => p.VerifyPassword(loginDTO.Password, user.PasswordHash)).Returns(false);

            // Act
            Func<Task> act = async () => await _authService.AuthenticateAsync(loginDTO);

            // Assert
            await act.Should().ThrowAsync<InvalidCredentialCustomException>()
                .WithMessage("Invalid credentials!");
        }
    }
}
