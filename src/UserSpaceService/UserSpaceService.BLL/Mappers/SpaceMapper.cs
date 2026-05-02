using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public static class SpaceMapper
{
    public static Space ToEntity(this SpaceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new Space
        {
            Id = dto.Id,
            Name = dto.Name,
            AvatarUrl = dto.AvatarUrl,
            Members = dto.Members.Select(x => x.ToEntity()).ToList(),
            Roles = dto.Roles.Select(x => x.ToEntity()).ToList()
        };
    }
    
    public static Space ToEntity(this SpaceDtoShort dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new Space
        {
            Id = dto.Id,
            Name = dto.Name,
            AvatarUrl = dto.AvatarUrl,
        };
    }

    public static SpaceDto ToDto(this Space entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDto
        {
            Id = entity.Id,
            Name = entity.Name,
            AvatarUrl = entity.AvatarUrl,
            Members = entity.Members.Select(x => x.ToDto()).ToList(),
            Roles = entity.Roles.Select(x => x.ToDto()).ToList()
        };
    }

    public static SpaceDtoShort ToShortDto(this Space entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new SpaceDtoShort
        {
            Id = entity.Id,
            Name = entity.Name,
            AvatarUrl = entity.AvatarUrl,
        };
    }
}

