using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.Mappers;

public class UserMapper(
    IEntityFactory<IUser> entityFactory,
    IMapper<IExternalLogin, ExternalLoginDto> externalLoginMapper)
        : IMapper<IUser, UserDto>
{
    public UserDto ToDto(IUser source)
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

    public IUser ToEntity(UserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var user = entityFactory.CreateEntity();
        user.Id = dto.Id;
        user.Username = dto.Username;
        user.Email = dto.Email;
        user.ExternalLogins = dto.ExternalLogins
            .Select(externalLoginMapper.ToEntity)
            .ToList();
        return user;
    }
}

public class UserShortMapper(
    IEntityFactory<IUser> entityFactory)
        : IMapper<IUser, UserDtoShort>
{
    public UserDtoShort ToDto(IUser source)
    {
        return new UserDtoShort
        {
            Id = source.Id,
            Username = source.Username,
            Email = source.Email
        };
    }

    public IUser ToEntity(UserDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var user = entityFactory.CreateEntity();
        user.Id = dto.Id;
        user.Username = dto.Username;
        user.Email = dto.Email;
        return user;
    }
}