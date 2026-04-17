using ChatNotifyService.ABS.Dtos;
using FluentValidation;

namespace ChatNotifyService.BLL.Validators;

public class MessageCreateDtoValidator : AbstractValidator<MessageCreateDto>
{
    public MessageCreateDtoValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("ChatId is required.");
        
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required.");
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(MaxContentLength).WithMessage("Content is invalid.")
            .MinimumLength(MinContentLength).WithMessage("Content is too short.");
    }
    
    private const int MaxContentLength = 1000;
    private const int MinContentLength = 1;
}
