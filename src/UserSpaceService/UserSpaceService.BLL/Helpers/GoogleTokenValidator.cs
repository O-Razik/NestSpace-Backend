using Google.Apis.Auth;

namespace UserSpaceService.BLL.Helpers;

public class GoogleTokenValidator : IExternalTokenValidator
{
    public async Task<(string? ProviderUserId, string? Email)> ValidateAsync(string token)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token);
        return (payload?.Subject, payload?.Email);
    }
}