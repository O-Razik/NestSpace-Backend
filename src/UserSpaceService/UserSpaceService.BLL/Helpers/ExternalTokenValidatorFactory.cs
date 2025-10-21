using Microsoft.Extensions.DependencyInjection;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.BLL.Helpers;

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