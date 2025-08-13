using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volcanion.Auth.Application.Features.Permissions.Queries;

namespace Volcanion.Auth.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get permissions list with pagination and filtering
    /// </summary>
    /// <param name="query">Query parameters</param>
    /// <returns>Paginated permissions list</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsQuery query)
    {
        var result = await _mediator.Send(query);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}
