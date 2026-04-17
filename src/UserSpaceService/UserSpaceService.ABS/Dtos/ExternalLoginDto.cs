using UserSpaceService.ABS.Models;

namespace UserSpaceService.ABS.Dtos;

public class ExternalLoginDto : ExternalLoginDtoShort
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
}

public class ExternalLoginDtoShort
{
    public Provider Provider { get; set; }
    
    public string ProviderKey { get; set; } = string.Empty;
}
