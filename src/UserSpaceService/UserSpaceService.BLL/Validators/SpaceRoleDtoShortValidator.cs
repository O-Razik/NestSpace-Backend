using FluentValidation;
using UserSpaceService.ABS.DTOs;

namespace UserSpaceService.BLL.Validators;

public class SpaceRoleDtoShortValidator : AbstractValidator<SpaceRoleDtoShort>
{
    public SpaceRoleDtoShortValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name is invalid.")
            .MinimumLength(3).WithMessage("Name is too short.");

        RuleFor(x => x.RolePermissions)
            .NotEmpty().WithMessage("RolePermissions cannot be empty.");
    }
}