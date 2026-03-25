using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.IRepositories;
using UserSpaceService.ABS.Models;
using UserSpaceService.DAL.Data;
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
        builder.Services.AddScoped<ExternalLogin>();
        builder.Services.AddScoped<SpaceRole>();
        builder.Services.AddScoped<User>();
        builder.Services.AddScoped<RefreshToken>();
        builder.Services.AddScoped<SpaceMember>();
        builder.Services.AddScoped<Space>();
        
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
