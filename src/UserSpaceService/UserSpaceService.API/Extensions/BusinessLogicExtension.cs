using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;
using UserSpaceService.ABS.IServices;
using UserSpaceService.API.Helpers;
using UserSpaceService.BLL.Helpers;
using UserSpaceService.BLL.Services;

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
}
