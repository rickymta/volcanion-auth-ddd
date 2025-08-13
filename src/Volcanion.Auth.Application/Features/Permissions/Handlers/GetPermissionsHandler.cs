using AutoMapper;
using MediatR;
using Volcanion.Auth.Application.Common;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Application.Features.Permissions.Queries;
using Volcanion.Auth.Application.Features.Users.Queries;
using Volcanion.Auth.Domain.Repositories;

namespace Volcanion.Auth.Application.Features.Permissions.Handlers;

public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, Result<PagedResult<PermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetPermissionsHandler(IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (permissions, totalCount) = await _permissionRepository.GetPagedPermissionsAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Resource,
                request.Action,
                request.SortBy,
                request.SortDirection
            );

            var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);
            
            var result = new PagedResult<PermissionDto>
            {
                Items = permissionDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Result<PagedResult<PermissionDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<PermissionDto>>.Failure($"Error retrieving permissions: {ex.Message}");
        }
    }
}
