using ForeignExchange.Application.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using Xunit;

namespace ForeignExchange.Tests.Services
{
    public class PasswordHasherServiceTests
    {
        private readonly PasswordHasherService _passwordHasherService;

        public PasswordHasherServiceTests()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"EncryptHashing:Pepper", "some-pepper"},
                    {"EncryptHashing:SaltSize", "16"},
                    {"EncryptHashing:IterationCount", "10000"},
                    {"EncryptHashing:KeySize", "32"}
                })
                .Build();

            _passwordHasherService = new PasswordHasherService(config);
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            string password = "mySecurePassword";

            // Act
            string hashedPassword = _passwordHasherService.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrEmpty(hashedPassword));
            Assert.Contains(".", hashedPassword); // A string deve conter um ponto separando salt e hash
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_ForValidPassword()
        {
            // Arrange
            string password = "mySecurePassword";
            string hashedPassword = _passwordHasherService.HashPassword(password);

            // Act
            bool result = _passwordHasherService.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForInvalidPassword()
        {
            // Arrange
            string password = "mySecurePassword";
            string hashedPassword = _passwordHasherService.HashPassword(password);
            string wrongPassword = "wrongPassword";

            // Act
            bool result = _passwordHasherService.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForMalformedHash()
        {
            // Arrange
            string password = "mySecurePassword";
            string malformedHash = "malformedHashString";

            // Act
            bool result = _passwordHasherService.VerifyPassword(password, malformedHash);

            // Assert
            Assert.False(result);
        }
    }


}
