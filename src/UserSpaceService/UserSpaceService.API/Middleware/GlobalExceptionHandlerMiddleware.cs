using System.Net;
using System.Text.Json;
using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Exceptions;
using UserSpaceService.ABS.IHelpers;

namespace UserSpaceService.API.Middleware;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IHostEnvironment environment,
    IDateTimeProvider dateTime)
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            NotFoundException notFoundEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundEx.Message
            },
            BadRequestException badRequestEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = badRequestEx.Message
            },
            UnauthorizedException unauthorizedEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = unauthorizedEx.Message
            },
            ForbiddenException forbiddenEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.Forbidden,
                Message = forbiddenEx.Message
            },
            ConflictException conflictEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = conflictEx.Message
            },
            ArgumentException argEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = argEx.Message
            },
            InvalidOperationException invalidOpEx => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = invalidOpEx.Message
            },
            _ => new ErrorResponseDto(dateTime)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred.",
                Details = environment.IsDevelopment() ? exception.Message : null
            }
        };

        context.Response.StatusCode = errorResponse.StatusCode;
        var json = JsonSerializer.Serialize(errorResponse, _options);
        await context.Response.WriteAsync(json);
    }
}
