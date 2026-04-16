using EventScheduleService.ABS.Dtos;
using FluentValidation;

namespace EventScheduleService.BLL.Validators;

public class SoloEventUpdateDtoValidator : AbstractValidator<SoloEventUpdateDto>
{
    public SoloEventUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id must be a valid GUID.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(MaxTitleLength)
            .WithMessage("Title must not exceed 100 characters.")
            .Matches(AllowedCharactersPattern)
            .WithMessage("Title contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage("Description must not exceed 500 characters.")
            .Matches(AllowedCharactersPattern)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description contains invalid characters.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be on or before end date.");

        RuleFor(x => x.TagIds)
            .NotNull()
            .WithMessage("Tag IDs are required.");

        RuleForEach(x => x.TagIds)
            .Must(id => id != Guid.Empty)
            .WithMessage("Tag ID must be a valid GUID.");
    }

    private const string AllowedCharactersPattern = @"^[a-zA-Z0-9\s\p{P}]*$";
    private const int MaxTitleLength = 100;
    private const int MaxDescriptionLength = 500;
}
