using AutoMapper;
using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Roles.Queries;
using Volcanion.Auth.Application.Features.Users.Queries;
using Volcanion.Auth.Domain.Repositories;

namespace Volcanion.Auth.Application.Features.Roles.Handlers;

public class GetRolesHandler : IRequestHandler<GetRolesQuery, Result<PagedResult<RoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetRolesHandler(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (roles, totalCount) = await _roleRepository.GetPagedRolesAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.SortBy,
                request.SortDirection
            );

            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            
            var result = new PagedResult<RoleDto>
            {
                Items = roleDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Result<PagedResult<RoleDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<RoleDto>>.Failure($"Error retrieving roles: {ex.Message}");
        }
    }
}
