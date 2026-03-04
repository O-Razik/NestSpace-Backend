using FluentValidation;
using UserSpaceService.ABS.DTOs;

namespace UserSpaceService.BLL.Validators;

public class CreateSpaceDtoValidator : AbstractValidator<CreateSpaceDto>
{
    public CreateSpaceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name is invalid.")
            .MinimumLength(3).WithMessage("Name is too short.");
        
        RuleForEach(x => x.MemberIds)
            .NotEmpty().WithMessage("MemberId cannot be empty.");
    }
}