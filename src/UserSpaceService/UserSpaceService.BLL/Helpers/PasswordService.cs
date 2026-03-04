using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.BLL.Helpers;

/// <summary>
/// Service for hashing and verifying user passwords using ASP.NET Core Identity's PasswordHasher.
/// </summary>
/// <param name="userFactory">Factory for creating IUser instances required by PasswordHasher.</param>
/// <param name="passwordHasher">The PasswordHasher instance for hashing and verifying passwords.</param>
/// <remarks>
/// This service abstracts the password hashing logic and allows for easy integration with the IUser model.
/// It uses the PasswordHasher from ASP.NET Core Identity, which implements a secure hashing algorithm (
/// PBKDF2) and includes salting and multiple iterations to protect against brute-force attacks.
/// </remarks>
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
