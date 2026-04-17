using EventScheduleService.ABS.Dtos;
using FluentValidation;

namespace EventScheduleService.BLL.Validators;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Title must not exceed {MaxNameLength} characters.")
            .Matches(AllowedCharactersPattern)
            .WithMessage("Title contains invalid characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.")
            .Matches(AllowedCharactersPattern)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description contains invalid characters.");
    }

    private const string AllowedCharactersPattern = @"^[a-zA-Z0-9\s\p{P}]*$";
    private const int MaxNameLength = 50;
    private const int MaxDescriptionLength = 200;
}
