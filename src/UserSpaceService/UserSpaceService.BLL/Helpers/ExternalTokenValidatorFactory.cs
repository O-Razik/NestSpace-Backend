using Microsoft.Extensions.DependencyInjection;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.BLL.Helpers;

/// <summary>
/// Factory for creating instances of IExternalTokenValidator based on the provider.
/// </summary> <param name="serviceProvider">The service provider for resolving dependencies.</param>
public class ExternalTokenValidatorFactory(IServiceProvider serviceProvider)
{
    public IExternalTokenValidator Create(Provider provider)
    {
        return provider switch
        {
            Provider.Google => serviceProvider.GetRequiredService<GoogleTokenValidator>(),
            Provider.Microsoft => serviceProvider.GetRequiredService<MicrosoftTokenValidator>(),
            _ => throw new NotSupportedException($"Unsupported provider: {provider}")
        };
    }
}
