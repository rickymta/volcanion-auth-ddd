using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;

namespace Volcanion.Auth.Application.Features.Users.Queries;

public class GetUserProfileQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
}

public class GetUsersQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
