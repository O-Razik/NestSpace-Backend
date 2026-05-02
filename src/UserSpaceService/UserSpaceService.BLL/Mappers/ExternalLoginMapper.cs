using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public static class ExternalLoginMapper
{
    public static ExternalLoginDto ToDto(this ExternalLogin source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new ExternalLoginDto
        {
            Id = source.Id,
            UserId = source.UserId,
            Provider = source.Provider,
            ProviderKey = source.ProviderKey
        };
    }

    public static ExternalLogin ToEntity(this ExternalLoginDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return new ExternalLogin
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Provider = dto.Provider,
            ProviderKey = dto.ProviderKey,
        };
    }
}
