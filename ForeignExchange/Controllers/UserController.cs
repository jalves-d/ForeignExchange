using ForeignExchange.Application.DTOs;
using ForeignExchange.Application.Interfaces;
using ForeignExchange.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UserController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <remarks>
    /// This endpoint allows a new user to register by providing necessary information.
    /// </remarks>
    /// <param name="registrationDto">The data transfer object containing user registration details.</param>
    /// <returns>A response indicating the success of the registration.</returns>
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user", OperationId = "Register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserDTO registrationDto)
    {
        try
        {
            await _userService.RegisterUserAsync(registrationDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete an exchange rate
    /// </summary>
    /// <remarks>
    /// This endpoint deletes an exchange rate for the specified currency pair.
    /// </remarks>
    /// <param name="baseCurrency">The base currency code.</param>
    /// <param name="quoteCurrency">The quote currency code.</param>
    /// <returns>Confirmation of the deleted exchange rate.</returns>
    [HttpDelete("{user}")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete an user", OperationId = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(string user)
    {
        try
        {
            await _userService.DeleteUserAsync(user);
            return Ok("User was deleted!");
        }
        catch (Exception ex)
        {
            return NotFound("User was not deleted due to error: " + ex.Message);
        }
    }

    /// <summary>
    /// Login a registered user
    /// </summary>
    /// <remarks>
    /// This endpoint allows a registered user to log in and receive a token for authenticated requests.
    /// </remarks>
    /// <param name="loginDto">The data transfer object containing user login details.</param>
    /// <returns>A token that can be used to validate authenticated requests.</returns>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login a registered user", OperationId = "Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        try
        {
            var token = await _authService.AuthenticateAsync(loginDto);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
