using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IServices;
using UserSpaceService.API.Helpers;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Mappers;
using UserSpaceService.BLL.Services;
using UserSpaceService.BLL.Validators;

namespace UserSpaceService.API.Extensions;

public static class BusinessLogicExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISpaceService, SpaceService>();
        
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<PasswordHasher<User>>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<IGetCurrentUser, GetCurrentUser>();
        builder.Services.AddScoped<GoogleTokenValidator>();
        builder.Services.AddScoped<MicrosoftTokenValidator>();
        builder.Services.AddScoped<ExternalTokenValidatorFactory>();
        
        return builder;
    }
    
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
    
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoShortValidator>();
        builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoShortValidator>();
        builder.Services.AddScoped<IValidator<AddSpaceMemberDto>, AddSpaceMemberDtoValidator>();
        builder.Services.AddScoped<IValidator<CreateSpaceDto>, CreateSpaceDtoValidator>();
        builder.Services.AddScoped<IValidator<SpaceRoleDtoShort>, SpaceRoleDtoShortValidator>();
        
        return builder;
    }
}
