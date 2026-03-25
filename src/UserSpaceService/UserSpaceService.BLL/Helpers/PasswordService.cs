using Microsoft.AspNetCore.Identity;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Helpers;

/// <summary>
/// Service for hashing and verifying user passwords using ASP.NET Core Identity's PasswordHasher.
/// </summary>
/// <param name="passwordHasher">The PasswordHasher instance for hashing and verifying passwords.</param>
/// <remarks>
/// This service abstracts the password hashing logic and allows for easy integration with the IUser model.
/// It uses the PasswordHasher from ASP.NET Core Identity, which implements a secure hashing algorithm (
/// PBKDF2) and includes salting and multiple iterations to protect against brute-force attacks.
/// </remarks>
public class PasswordService(
    PasswordHasher<User> passwordHasher)
{

    public string HashPassword(string password)
    {
        var user = new User(); // Create a dummy user instance for hashing
        return passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var user = new User(); // Create a dummy user instance for verification
        var result = passwordHasher.VerifyHashedPassword(user, hash, password);
        return result == PasswordVerificationResult.Success;
    }
}
