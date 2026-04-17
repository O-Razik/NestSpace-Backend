using EventScheduleService.ABS.Dtos;
using FluentValidation;

namespace EventScheduleService.BLL.Validators;

public class RegularEventCreateDtoValidator : AbstractValidator<RegularEventCreateDto>
{
    public RegularEventCreateDtoValidator()
    {
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

        RuleFor(x => x.StartTime)
            .NotEqual(TimeOnly.MinValue)
            .WithMessage("Start time is required.");

        RuleForEach(x => x.Tags)
            .Must(tag => tag.Id != Guid.Empty)
            .WithMessage("Tag ID must be a valid GUID.")
            .Must((dto, tag) => tag.SpaceId == dto.SpaceId)
            .WithMessage("Tag must exist in space.")
            .When(x => x.Tags != null);
        
        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .WithMessage("Category ID must be a valid GUID.");
    }

    private const string AllowedCharactersPattern = @"^[a-zA-Z0-9\s\p{P}]*$";
    private const int MaxTitleLength = 100;
    private const int MaxDescriptionLength = 500;
}
