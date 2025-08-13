using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Users.Queries;

namespace Volcanion.Auth.Application.Features.Permissions.Queries;

public class GetPermissionsQuery : IRequest<Result<PagedResult<PermissionDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Resource { get; set; }
    public string? Action { get; set; }
    public string? SortBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "asc";
}
