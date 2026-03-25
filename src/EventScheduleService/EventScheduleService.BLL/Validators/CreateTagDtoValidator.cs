using EventScheduleService.ABS.Dto;
using FluentValidation;

namespace EventScheduleService.BLL.Validators;

public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(MaxNameLength)
            .WithMessage($"Title must not exceed {MaxNameLength} characters.")
            .Matches(AllowedCharactersPattern)
            .WithMessage("Title contains invalid characters.");
        
        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Color is required.")
            .Matches(@"^#([0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")
            .WithMessage("Color must be a valid hex code.")
            .When(x => !string.IsNullOrWhiteSpace(x.Color))
            .WithMessage("Color contains invalid characters.");
    }

    private const string AllowedCharactersPattern = @"^[a-zA-Z0-9\s\p{P}]*$";
    private const int MaxNameLength = 50;
}