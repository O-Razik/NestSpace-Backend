using FluentValidation;
using UserSpaceService.ABS.Dtos;

namespace UserSpaceService.BLL.Validators;

public class RegisterDtoShortValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoShortValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(MaxNameLength).WithMessage("Username is invalid.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(MaxEmailLength).WithMessage("Email is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(HasUpperCase).WithMessage("Password must contain at least one uppercase letter.")
            .Must(HasLowerCase).WithMessage("Password must contain at least one lowercase letter.")
            .Must(HasDigit).WithMessage("Password must contain at least one digit.")
            .Must(HasSpecialChar).WithMessage("Password must contain at least one special character.")
            .MinimumLength(MinPasswordLength).WithMessage("Password must be at least 8 characters.")
            .NotEqual(x => x.Username).WithMessage("Password cannot be the same as the username.")
            .MaximumLength(MaxPasswordLength);
    }
    
    private static bool HasUpperCase(string password) =>
        password.Any(char.IsUpper);

    private static bool HasLowerCase(string password) =>
        password.Any(char.IsLower);

    private static bool HasDigit(string password) =>
        password.Any(char.IsDigit);

    private static bool HasSpecialChar(string password) =>
        password.Any(ch => !char.IsLetterOrDigit(ch));
    
    private const int MaxNameLength = 100;
    private const int MaxEmailLength = 255;
    private const int MaxPasswordLength = 255;
    private const int MinPasswordLength = 8;
}
