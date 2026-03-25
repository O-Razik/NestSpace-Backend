using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public class UserMapper(
    IMapper<ExternalLogin, ExternalLoginDto> externalLoginMapper)
        : IMapper<User, UserDto>
{
    public UserDto ToDto(User source)
    {
        return new UserDto
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email,
            ExternalLogins = source.ExternalLogins
                .Select(externalLoginMapper.ToDto).ToList()
        };
    }

    public User ToEntity(UserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email,
            ExternalLogins = dto.ExternalLogins
                .Select(externalLoginMapper.ToEntity)
                .ToList()
        };
    }
}

public class UserShortMapper
        : IMapper<User, UserDtoShort>
{
    public UserDtoShort ToDto(User source)
    {
        return new UserDtoShort
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email
        };
    }

    public User ToEntity(UserDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new User
        {
            Id = dto.Id,
            Username = dto.Username,
            Email = dto.Email
        };
    }
}
