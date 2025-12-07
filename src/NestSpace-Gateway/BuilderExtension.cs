using Ocelot.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

namespace NestSpace_Gateway;

public static class BuilderExtension
{
    public static WebApplicationBuilder AddOcelot(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Services.AddOcelot();
        return builder;
    }
    
    /*
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console()
            .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
            {
                clientConfiguration.Username = "guest";
                clientConfiguration.Password = "guest";
                clientConfiguration.VHost = "/";
                clientConfiguration.Hostnames.Add("localhost");
                clientConfiguration.Exchange = "logs_exchange";
                clientConfiguration.ExchangeType = "direct";
                sinkConfiguration.TextFormatter = new JsonFormatter();
            })
            .CreateLogger();

        builder.Host.UseSerilog();
        
        return builder;
    }
    */
    
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "NestSpace.Gateway")
        );
        
        return builder;
    }
}
