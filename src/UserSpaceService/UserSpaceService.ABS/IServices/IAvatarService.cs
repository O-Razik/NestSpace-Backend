namespace UserSpaceService.ABS.IServices;

public interface IAvatarService
{
    Task<string> SaveAvatarAsync(Stream fileStream, string fileName, long fileSize, string category, Guid entityId);
    Task DeleteAvatarAsync(string? avatarUrl);
}