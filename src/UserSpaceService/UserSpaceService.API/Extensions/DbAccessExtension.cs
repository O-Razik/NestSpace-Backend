using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IModels;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.DAL.Data;
using UserSpaceService.DAL.Models;
using UserSpaceService.DAL.Repositories;

namespace UserSpaceService.API.Extensions;

public static class DbAccessExtension
{
    public static WebApplicationBuilder AddSqlDbContext(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<UserSpaceDbContext>(options => options.UseNpgsql(connectionString));
        
        return builder;
    }
    
    public static WebApplicationBuilder AddModels(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IExternalLogin, ExternalLogin>();
        builder.Services.AddScoped<ISpaceRole, SpaceRole>();
        builder.Services.AddScoped<IUser, User>();
        builder.Services.AddScoped<IRefreshToken, RefreshToken>();
        builder.Services.AddScoped<ISpaceMember, SpaceMember>();
        builder.Services.AddScoped<ISpace, Space>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISpaceRoleRepository, SpaceRoleRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
        builder.Services.AddScoped<ISpaceMemberRepository, SpaceMemberRepository>();
        
        return builder;
    }
}
