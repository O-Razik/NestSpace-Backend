namespace UserSpaceService.BLL.Helpers;

/// <summary>
/// Interface for validating external authentication tokens from providers like Google and Microsoft.
/// </summary>
public interface IExternalTokenValidator
{
    Task<(string? ProviderUserId, string? Email)> ValidateAsync(string token);
}
