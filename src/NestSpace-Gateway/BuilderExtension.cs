using Ocelot.DependencyInjection;

namespace NestSpace_Gateway;

public static class BuilderExtension
{
    public static WebApplicationBuilder AddOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot();
        
        return builder;
    }
}