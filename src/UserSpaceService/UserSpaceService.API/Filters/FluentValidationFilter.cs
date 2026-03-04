using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserSpaceService.API.Filters;

public sealed class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validator = context.HttpContext.RequestServices
                .GetService(typeof(IValidator<>).MakeGenericType(argument.GetType())) as IValidator;

            if (validator is null) continue;

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument));

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    errors.TryAdd(error.PropertyName, []);
                    errors[error.PropertyName] =
                        errors[error.PropertyName].Append(error.ErrorMessage).ToArray();
                }
            }
        }

        if (errors.Count > 0)
        {
            context.Result = new BadRequestObjectResult(
                new ValidationProblemDetails(errors)
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status400BadRequest
                });
            return;
        }

        await next();
    }
}