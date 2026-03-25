using System.Reflection;
using Microsoft.OpenApi.Models;

namespace UserSpaceService.API.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger with JWT Bearer authentication support
    /// </summary>
    public static IServiceCollection AddSwaggerWithJwtAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // API Info
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UserSpace API",
                Version = "v1",
                Description = "API for managing users and spaces",
                Contact = new OpenApiContact
                {
                    Name = "Your Name",
                    Email = "your.email@example.com"
                }
            });

            // JWT Bearer Security
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });

            // XML Comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware
    /// </summary>
    public static IApplicationBuilder UseSwaggerWithUi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserSpace API v1");
            c.RoutePrefix = string.Empty; // Swagger UI at root (optional)
            c.DocumentTitle = "UserSpace API Documentation";
            c.DisplayRequestDuration();
        });

        return app;
    }
}