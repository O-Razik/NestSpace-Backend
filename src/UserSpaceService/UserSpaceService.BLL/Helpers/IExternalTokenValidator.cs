namespace UserSpaceService.BLL.Helpers;

public interface IExternalTokenValidator
{
    Task<(string? ProviderUserId, string? Email)> ValidateAsync(string token);
}