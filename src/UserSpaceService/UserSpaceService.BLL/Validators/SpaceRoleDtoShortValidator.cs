using FluentValidation;
using UserSpaceService.ABS.DTOs;

namespace UserSpaceService.BLL.Validators;

public class SpaceRoleDtoShortValidator : AbstractValidator<SpaceRoleDtoShort>
{
    public SpaceRoleDtoShortValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxStringLength).WithMessage("Name is invalid.")
            .MinimumLength(MinStringLength).WithMessage("Name is too short.");

        RuleFor(x => x.RolePermissions)
            .NotEmpty().WithMessage("RolePermissions cannot be empty.");
    }
    
    private const int MaxStringLength = 255;
    private const int MinStringLength = 3;
}
