using Ocelot.DependencyInjection;

namespace Gateway.API.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
        builder.Services.AddOcelot();
        
        return builder;
    }
}