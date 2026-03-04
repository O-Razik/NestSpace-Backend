using FluentValidation;
using UserSpaceService.ABS.Filters;

namespace UserSpaceService.BLL.Validators;

public class SpaceFilterValidator : AbstractValidator<SpaceFilter>
{
    public SpaceFilterValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Name is invalid.");
        
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1.");
    }
}
