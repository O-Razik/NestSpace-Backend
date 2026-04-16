using FluentValidation;
using UserSpaceService.ABS.Dtos;

namespace UserSpaceService.BLL.Validators;

public class SpaceCreateDtoValidator : AbstractValidator<CreateSpaceDto>
{
    public SpaceCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxNameLength).WithMessage("Name is invalid.")
            .MinimumLength(MinNameLength).WithMessage("Name is too short.");
        
        RuleForEach(x => x.MemberIds)
            .NotEmpty().WithMessage("MemberId cannot be empty.");
    }
    
    private const int MaxNameLength = 200;
    private const int MinNameLength = 3;
}
