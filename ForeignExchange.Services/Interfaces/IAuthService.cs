using ForeignExchange.Application.DTOs;

public interface IAuthService
{
    Task<string> AuthenticateAsync(LoginDTO loginDTO);
}
