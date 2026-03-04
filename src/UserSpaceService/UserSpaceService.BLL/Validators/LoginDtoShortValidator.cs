using FluentValidation;
using UserSpaceService.ABS.DTOs;

namespace UserSpaceService.BLL.Validators;

public class LoginDtoShortValidator : AbstractValidator<LoginDtoShort>
{
    public LoginDtoShortValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}