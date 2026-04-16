using ChatNotifyService.ABS.Dtos;
using FluentValidation;

namespace ChatNotifyService.BLL.Validators;

public class ChatCreateDtoValidator : AbstractValidator<ChatCreateDto>
{
    public ChatCreateDtoValidator()
    {
        RuleFor(x => x.SpaceId)
            .NotEmpty().WithMessage("SpaceId is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxNameLength).WithMessage("Name is invalid.")
            .MinimumLength(MinNameLength).WithMessage("Name is too short.");

        RuleFor(x => x.Members)
            .NotEmpty().WithMessage("Member cannot be empty.");
        
        RuleForEach(x => x.Members).ChildRules(member =>
        {
            member.RuleFor(m => m.MemberId)
                .NotEmpty().WithMessage("MemberId cannot be empty.");
        });
    }
    
    private const int MaxNameLength = 200;
    private const int MinNameLength = 3;
}
