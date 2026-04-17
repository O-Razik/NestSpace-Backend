using UserSpaceService.ABS.Dtos;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.BLL.Mappers;

public class ExternalLoginMapper
    : IMapper<ExternalLogin, ExternalLoginDto>
{
    public ExternalLoginDto ToDto(ExternalLogin source)
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

    public ExternalLogin ToEntity(ExternalLoginDto dto)
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
