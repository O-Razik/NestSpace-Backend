namespace UserSpaceService.ABS.Dtos;

public class UserDtoShort
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? AvatarUrl { get; set; }
}

public class UserDto : UserDtoShort
{
    public ICollection<ExternalLoginDto> ExternalLogins { get; set; } = new List<ExternalLoginDto>();
}
