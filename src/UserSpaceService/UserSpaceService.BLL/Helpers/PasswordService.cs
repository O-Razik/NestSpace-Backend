using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.BLL.Helpers;

public class PasswordService(
    IEntityFactory<IUser> userFactory,
    PasswordHasher<IUser> passwordHasher)
{

    public string HashPassword(string password)
    {
        var user = userFactory.CreateEntity();
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var user = userFactory.CreateEntity();
        var result = passwordHasher.VerifyHashedPassword(user, hash, password);
        return result == PasswordVerificationResult.Success;
    }
}