using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace UserSpaceService.API.Extensions;

public static class AuthenticationExtension
{
    /// <summary>
    /// Adds JWT Bearer authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("Jwt:Key is not configured");
        var jwtIssuer = configuration["Jwt:Issuer"] 
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured");
        var jwtAudience = configuration["Jwt:Audience"] 
            ?? throw new InvalidOperationException("Jwt:Audience is not configured");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    /// <summary>
    /// Adds external authentication providers (Google, Microsoft)
    /// </summary>
    public static IServiceCollection AddExternalAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authBuilder = services.AddAuthentication();

        // Google
        var googleClientId = configuration["AuthExternal:Google:ClientId"];
        var googleClientSecret = configuration["AuthExternal:Google:ClientSecret"];
        
        if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
        {
            authBuilder.AddGoogle(options =>
            {
                options.ClientId = googleClientId;
                options.ClientSecret = googleClientSecret;
            });
        }

        // Microsoft
        var microsoftClientId = configuration["AuthExternal:Microsoft:ClientId"];
        var microsoftClientSecret = configuration["AuthExternal:Microsoft:ClientSecret"];
        
        if (!string.IsNullOrEmpty(microsoftClientId) && !string.IsNullOrEmpty(microsoftClientSecret))
        {
            authBuilder.AddMicrosoftAccount(options =>
            {
                options.ClientId = microsoftClientId;
                options.ClientSecret = microsoftClientSecret;
            });
        }

        return services;
    }

    /// <summary>
    /// Adds all authentication services (JWT + External providers)
    /// </summary>
    public static IServiceCollection AddAllAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddJwtAuthentication(configuration);
        services.AddExternalAuthentication(configuration);
        return services;
    }
}
