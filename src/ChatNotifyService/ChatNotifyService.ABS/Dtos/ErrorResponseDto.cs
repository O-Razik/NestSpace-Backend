using ChatNotifyService.ABS.IHelpers;

namespace ChatNotifyService.ABS.Dtos;

public class ErrorResponseDto(IDateTimeProvider dateTime)
{
    public int StatusCode { get; set; }
    
    public string Message { get; set; } = string.Empty;
    
    public string? Details { get; set; }
    
    public DateTimeOffset Timestamp { get; set; } = dateTime.UtcNow;
}
