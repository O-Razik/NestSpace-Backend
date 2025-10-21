using System.Net.Http.Headers;
using System.Text.Json;

namespace UserSpaceService.BLL.Helpers;

public class MicrosoftTokenValidator : IExternalTokenValidator
{
    private static readonly HttpClient HttpClient = new();

    public async Task<(string? ProviderUserId, string? Email)> ValidateAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await HttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return (null, null);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        var id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
        var email = root.TryGetProperty("mail", out var mailProp) && !string.IsNullOrEmpty(mailProp.GetString())
            ? mailProp.GetString()
            : root.TryGetProperty("userPrincipalName", out var upnProp)
                ? upnProp.GetString()
                : null;

        return (id, email);
    }
}