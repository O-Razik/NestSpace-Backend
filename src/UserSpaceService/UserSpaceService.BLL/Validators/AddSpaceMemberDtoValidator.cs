using FluentValidation;
using UserSpaceService.ABS.Dtos;

namespace UserSpaceService.BLL.Validators;

public class AddSpaceMemberDtoValidator : AbstractValidator<AddSpaceMemberDto>
{
    public AddSpaceMemberDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
        
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}
