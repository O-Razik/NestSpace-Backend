using FluentValidation;
using UserSpaceService.ABS.Dtos;

namespace UserSpaceService.BLL.Validators;

public class LoginDtoShortValidator : AbstractValidator<LoginDto>
{
    public LoginDtoShortValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is invalid.")
            .MaximumLength(MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
    
    private const int MaxEmailLength = 256;
}
