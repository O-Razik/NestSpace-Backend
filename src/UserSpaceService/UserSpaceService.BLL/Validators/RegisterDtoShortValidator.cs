using FluentValidation;
using UserSpaceService.ABS.DTOs;

namespace UserSpaceService.BLL.Validators;

public class RegisterDtoShortValidator : AbstractValidator<RegisterDtoShort>
{
    public RegisterDtoShortValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(HasUpperCase).WithMessage("Password must contain at least one uppercase letter.")
            .Must(HasLowerCase).WithMessage("Password must contain at least one lowercase letter.")
            .Must(HasDigit).WithMessage("Password must contain at least one digit.")
            .Must(HasSpecialChar).WithMessage("Password must contain at least one special character.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .NotEqual(x => x.Username).WithMessage("Password cannot be the same as the username.")
            .MaximumLength(100);
    }
    
    private bool HasUpperCase(string password) =>
        password.Any(char.IsUpper);

    private bool HasLowerCase(string password) =>
        password.Any(char.IsLower);

    private bool HasDigit(string password) =>
        password.Any(char.IsDigit);

    private bool HasSpecialChar(string password) =>
        password.Any(ch => !char.IsLetterOrDigit(ch));
}