using ForeignExchange.Application.DTOs;

public interface IAuthService
{
    Task<string> AuthenticateAsync(UserDTO loginDTO);
}
