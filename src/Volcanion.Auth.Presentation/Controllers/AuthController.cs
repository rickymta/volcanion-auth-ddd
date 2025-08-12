using MediatR;
using Microsoft.AspNetCore.Mvc;
using Volcanion.Auth.Application.Features.Authentication.Commands;

namespace Volcanion.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="command">Registration details</param>
    /// <returns>Authentication result with tokens</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Login with email/phone and password
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>Authentication result with tokens</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // Set device info from request
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();
        command.DeviceInfo = ExtractDeviceInfo();

        var result = await _mediator.Send(command);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    /// <param name="command">Logout request</param>
    /// <returns>Success status</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result)
        {
            return BadRequest(new { message = "Failed to logout" });
        }

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="command">Refresh token request</param>
    /// <returns>New token pair</returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result == null)
        {
            return BadRequest(new { message = "Invalid or expired refresh token" });
        }

        return Ok(result);
    }

    private string ExtractDeviceInfo()
    {
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        
        // Simple device detection
        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            return "Mobile Device";
        if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            return "Tablet";
        if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
            return "Windows Desktop";
        if (userAgent.Contains("Mac", StringComparison.OrdinalIgnoreCase))
            return "Mac Desktop";
        if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            return "Linux Desktop";
        
        return "Unknown Device";
    }
}
