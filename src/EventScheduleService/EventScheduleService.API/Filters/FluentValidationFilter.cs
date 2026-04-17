using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventScheduleService.API.Filters;

/// <summary>
/// A filter that validates action arguments using FluentValidation validators.
/// </summary>
public sealed class FluentValidationFilter : IAsyncActionFilter
{
    /// <summary>
    /// Validates action arguments using FluentValidation validators.
    /// If validation fails, returns a 400 Bad Request response with details about the validation errors.
    /// </summary>
    /// <param name="context"> The context of the action being executed, which contains information about the action arguments and the HTTP request.</param>
    /// <param name="next"> A delegate that represents the next action filter or the action itself to be executed if validation succeeds.</param>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validator = context.HttpContext.RequestServices
                .GetService(typeof(IValidator<>).MakeGenericType(argument.GetType())) as IValidator;

            if (validator is null)
            {
                continue;
            }

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument));

            if (result.IsValid)
            {
                continue;
            }
            foreach (var error in result.Errors)
            {
                errors.TryAdd(error.PropertyName, []);
                errors[error.PropertyName] =
                    errors[error.PropertyName].Append(error.ErrorMessage).ToArray();
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
