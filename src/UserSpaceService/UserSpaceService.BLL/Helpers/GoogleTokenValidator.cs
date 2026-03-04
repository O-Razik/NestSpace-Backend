using Google.Apis.Auth;

namespace UserSpaceService.BLL.Helpers;


/// <summary>
/// Validates Google ID tokens using the Google.Apis.Auth library.
/// </summary>
public class GoogleTokenValidator : IExternalTokenValidator
{
    public async Task<(string? ProviderUserId, string? Email)> ValidateAsync(string token)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token);
        return (payload?.Subject, payload?.Email);
    }
}
