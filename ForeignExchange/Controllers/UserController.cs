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
    /// This endpoint allows a new user to register by providing necessary information and provides authentication.
    /// </remarks>
    /// <param name="registrationDto">The data transfer object containing user registration details.</param>
    /// <returns>A token that can be used to validate authenticated requests.</returns>
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user", OperationId = "Register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserDTO registrationDto)
    {
        try
        {
            await _userService.RegisterUserAsync(registrationDto);
            var loginDto = new LoginDTO { Login = registrationDto.Username, Password = registrationDto.Password };
            var token = await _authService.AuthenticateAsync(loginDto);

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a specified user
    /// </summary>
    /// <remarks>
    /// This endpoint deletes a specified user.
    /// </remarks>
    /// <param name="user">The user email or username.</param>
    /// <returns>Confirmation of the deleted the user.</returns>
    [HttpDelete("{user}")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete a specified user", OperationId = "DeleteUser")]
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
    /// Update an existing exchange rate
    /// </summary>
    /// <remarks>
    /// This endpoint updates an existing email with new data.
    /// </remarks>
    /// <param name="newUsername">The new username to your user.</param>
    /// <returns>Confirmation of the updated username.</returns>
    [HttpPut("update-username")]
    [Authorize]
    [SwaggerOperation(Summary = "Update user username", OperationId = "UpdateUsername")]
    public async Task<IActionResult> UpdateUsername(string newUsername)
    {
        try
        {
            await _userService.UpdateUsernameAsync(newUsername);
            return Ok("Username was updated!");
        }
        catch (Exception ex)
        {
            return NotFound("Update not completed due to error: " + ex.Message);
        }
    }

    /// <summary>
    /// Update user email
    /// </summary>
    /// <remarks>
    /// This endpoint updates an existing email with new data.
    /// </remarks>
    /// <param name="newEmail">The new email to your user.</param>
    /// <returns>Confirmation of the updated email.</returns>
    [HttpPut("update-email")]
    [Authorize]
    [SwaggerOperation(Summary = "Update user email", OperationId = "UpdateEmail")]
    public async Task<IActionResult> UpdateEmail(string newEmail)
    {
        try
        {
            await _userService.UpdateEmailAsync(newEmail);
            return Ok("Email was updated!");
        }
        catch (Exception ex)
        {
            return NotFound("Update not completed due to error: " + ex.Message);
        }
    }

    /// <summary>
    /// Delete my own registration
    /// </summary>
    /// <remarks>
    /// This endpoint deletes the user registration.
    /// </remarks>
    /// <returns>Confirmation of the deleted my registration.</returns>
    [HttpDelete]
    [Authorize]
    [SwaggerOperation(Summary = "Delete my own registration", OperationId = "DeleteMyRegistration")]
    public async Task<IActionResult> DeleteMyRegistration()
    {
        try
        {
            await _userService.DeleteUserAsync();
            return Ok("Your user was deleted!");
        }
        catch (Exception ex)
        {
            return NotFound("Registration was not deleted due to error: " + ex.Message);
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
