using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public static class UserMapper
{
    public static User ToEntity(this UserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            AvatarUrl = dto.AvatarUrl,
            ExternalLogins = dto.ExternalLogins
                .Select(x => x.ToEntity())
                .ToList()
        };
    }

    public static User ToEntity(this UserDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            AvatarUrl = dto.AvatarUrl
        };
    }
    
    public static UserDto ToDto(this User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            AvatarUrl = source.AvatarUrl,
            ExternalLogins = source.ExternalLogins
                .Select(x => x.ToDto()).ToList()
        };
    }
    
    public static UserDtoShort ToShortDto(this User source)
    {
        return new UserDtoShort
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            AvatarUrl = source.AvatarUrl
        };
    }
}
