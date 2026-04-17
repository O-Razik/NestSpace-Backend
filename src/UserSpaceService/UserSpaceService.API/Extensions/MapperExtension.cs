using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.BLL.Mappers;

namespace UserSpaceService.API.Extensions;

public static class MapperExtension 
{
    public static WebApplicationBuilder AddMappersAndFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMapper<User, UserDto>, UserMapper>();
        builder.Services.AddScoped<IMapper<User, UserDtoShort>, UserShortMapper>();
        builder.Services.AddScoped<IMapper<SpaceMember, SpaceMemberDto>, SpaceMemberMapper>();
        builder.Services.AddScoped<IMapper<SpaceMember, SpaceMemberDtoShort>, SpaceMemberShortMapper>();
        builder.Services.AddScoped<IMapper<Space, SpaceDto>, SpaceMapper>();
        builder.Services.AddScoped<IMapper<Space, SpaceDtoShort>, SpaceShortMapper>();
        builder.Services.AddScoped<IMapper<SpaceRole, SpaceRoleDto>, SpaceRoleMapper>();
        builder.Services.AddScoped<IMapper<ExternalLogin, ExternalLoginDto>, ExternalLoginMapper>(); 
        
        return builder;
    }
}
