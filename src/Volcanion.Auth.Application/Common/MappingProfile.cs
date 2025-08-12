using AutoMapper;
using Volcanion.Auth.Application.DTOs;
using Volcanion.Auth.Domain.Entities;
using Volcanion.Auth.Domain.ValueObjects;

namespace Volcanion.Auth.Application.Common;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom<PhoneNumberResolver>())
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => 
                src.UserRoles.SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))));

        CreateMap<Role, string>()
            .ConvertUsing(src => src.Name);

        CreateMap<Permission, string>()
            .ConvertUsing(src => src.Name);
    }
}

public class PhoneNumberResolver : IValueResolver<User, UserDto, string?>
{
    public string? Resolve(User source, UserDto destination, string? destMember, ResolutionContext context)
    {
        return source.PhoneNumber is not null ? source.PhoneNumber.Value : null;
    }
}
