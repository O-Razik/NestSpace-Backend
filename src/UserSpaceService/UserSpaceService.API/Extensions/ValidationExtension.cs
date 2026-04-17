using FluentValidation;
using UserSpaceService.ABS.Dtos;
using UserSpaceService.BLL.Validators;

namespace UserSpaceService.API.Extensions;

public static class ValidationExtension 
{
    
    public static WebApplicationBuilder AddValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoShortValidator>();
        builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoShortValidator>();
        builder.Services.AddScoped<IValidator<AddSpaceMemberDto>, AddSpaceMemberDtoValidator>();
        builder.Services.AddScoped<IValidator<CreateSpaceDto>, SpaceCreateDtoValidator>();
        builder.Services.AddScoped<IValidator<SpaceRoleDtoShort>, SpaceRoleDtoShortValidator>();
        
        return builder;
    }
}
