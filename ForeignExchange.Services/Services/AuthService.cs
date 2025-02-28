using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ForeignExchange.Infrastructure.Interfaces;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.DTOs;
using ForeignExchange.Domain.Entities;
using ForeignExchange.Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace ForeignExchange.Application.Services
{
    public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IPasswordHasherService passwordHasherService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _configuration = configuration;
    }


    public async Task<string> AuthenticateAsync(LoginDTO loginDTO)
    {
        User user = null;

        if (loginDTO.Login != null)
        {
            user = await _userRepository.GetUserByUsernameAsync(loginDTO.Login);
            if (user == null)
                user = await _userRepository.GetUserByEmailAsync(loginDTO.Login);
        }

        if (user != null && _passwordHasherService.VerifyPassword(loginDTO.Password, user.PasswordHash))
        {
            var claim = new[] 
            { 
                new Claim("id", user.Id.ToString()), 
                new Claim("username", user.Username), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256Signature);

            var expiration = DateTime.UtcNow.AddMinutes(10);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claim,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        else
        {
            throw new InvalidCredentialCustomException("Invalid credentials!");
        }
    }
    }
}
