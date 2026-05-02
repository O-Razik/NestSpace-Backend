using UserSpaceService.ABS.IServices;

namespace UserSpaceService.API.Services;

public class AvatarService(IWebHostEnvironment env) : IAvatarService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public async Task<string> SaveAvatarAsync(Stream fileStream, string fileName, long fileSize, string category, Guid entityId)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed. Allowed types: jpg, jpeg, png, webp.");

        if (fileSize > MaxFileSizeBytes)
            throw new InvalidOperationException("File size cannot exceed 5 MB.");

        var avatarDir = Path.Combine(GetWebRootPath(), "avatars", category);
        Directory.CreateDirectory(avatarDir);

        var savedFileName = $"{entityId}{ext}";
        var fullPath = Path.Combine(avatarDir, savedFileName);

        await using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output);

        return $"/avatars/{category}/{savedFileName}";
    }

    public Task DeleteAvatarAsync(string? avatarUrl)
    {
        if (string.IsNullOrEmpty(avatarUrl))
            return Task.CompletedTask;

        var relativePath = avatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(GetWebRootPath(), relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string GetWebRootPath()
    {
        var webRoot = env.WebRootPath;
        if (string.IsNullOrEmpty(webRoot))
        {
            webRoot = Path.Combine(env.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(webRoot);
        }
        return webRoot;
    }
}