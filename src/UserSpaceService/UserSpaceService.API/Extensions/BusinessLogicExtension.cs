using FluentValidation;
using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IServices;
using UserSpaceService.ABS.Mappers;
using UserSpaceService.BLL.DTOs;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Mappers;
using UserSpaceService.BLL.Services;
using UserSpaceService.BLL.Validators;
using UserSpaceService.DAL.Helpers;

namespace UserSpaceService.API.Extensions;

public static class BusinessLogicExtension
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ISpaceService, SpaceService>();
        
        builder.Services.AddScoped<IPasswordHasher<IUser>, PasswordHasher<IUser>>();
        builder.Services.AddScoped<PasswordService>();
        builder.Services.AddScoped<GoogleTokenValidator>();
        builder.Services.AddScoped<MicrosoftTokenValidator>();
        builder.Services.AddScoped<ExternalTokenValidatorFactory>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddMappersAndFactories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEntityFactory<IUser>, UserFactory>();
        builder.Services.AddScoped<IEntityFactory<IExternalLogin>, ExternalLoginFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpace>, SpaceFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpaceMember>, SpaceMemberFactory>();
        builder.Services.AddScoped<IEntityFactory<ISpaceRole>, SpaceRoleFactory>();
    
        builder.Services.AddScoped<IMapper<IUser, UserDto>, UserMapper>();
        builder.Services.AddScoped<IMapper<IUser, UserDtoShort>, UserShortMapper>();
        builder.Services.AddScoped<IMapper<ISpaceMember, SpaceMemberDto>, SpaceMemberMapper>();
        builder.Services.AddScoped<IMapper<ISpaceMember, SpaceMemberDtoShort>, SpaceMemberShortMapper>();
        builder.Services.AddScoped<IMapper<ISpace, SpaceDto>, SpaceMapper>();
        builder.Services.AddScoped<IMapper<ISpace, SpaceDtoShort>, SpaceShortMapper>();
        builder.Services.AddScoped<IMapper<ISpaceRole, SpaceRoleDto>, SpaceRoleMapper>();
        builder.Services.AddScoped<IMapper<IExternalLogin, ExternalLoginDto>, ExternalLoginMapper>(); 
        
        return builder;
    }
    
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoShortValidator>();
        builder.Services.AddScoped<IValidator<RegisterDtoShort>, RegisterDtoShortValidator>();
        builder.Services.AddScoped<IValidator<AddSpaceMemberDto>, AddSpaceMemberDtoValidator>();
        builder.Services.AddScoped<IValidator<CreateSpaceDto>, CreateSpaceDtoValidator>();
        builder.Services.AddScoped<IValidator<SpaceRoleDtoShort>, SpaceRoleDtoShortValidator>();
        
        return builder;
    }
}