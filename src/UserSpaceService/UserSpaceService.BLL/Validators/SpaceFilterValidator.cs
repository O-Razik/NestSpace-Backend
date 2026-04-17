using FluentValidation;
using UserSpaceService.ABS.Filters;

namespace UserSpaceService.BLL.Validators;

public class SpaceFilterValidator : AbstractValidator<SpaceFilter>
{
    public SpaceFilterValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(MaxStringLength).WithMessage("Name is invalid.");
        
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(MinPageNumber).WithMessage("PageNumber must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(MinPageSize).WithMessage("PageSize must be greater than or equal to 1.");
    }
    
    private const int MaxStringLength = 255;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;
}
