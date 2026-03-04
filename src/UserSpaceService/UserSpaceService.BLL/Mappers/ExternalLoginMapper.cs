using UserSpaceService.ABS.DTOs;
using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.ABS.Mappers;

public class ExternalLoginMapper(IEntityFactory<IExternalLogin> entityFactory)
    : IMapper<IExternalLogin, ExternalLoginDto>
{
    public ExternalLoginDto ToDto(IExternalLogin source)
    {
        return new ExternalLoginDto
        {
            Id = source.Id,
            UserId = source.UserId,
            Provider = source.Provider,
            ProviderKey = source.ProviderKey
        };
    }

    public IExternalLogin ToEntity(ExternalLoginDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var externalLogin = entityFactory.CreateEntity();
        externalLogin.Id = dto.Id;
        externalLogin.UserId = dto.UserId;
        externalLogin.Provider = dto.Provider;
        externalLogin.ProviderKey = dto.ProviderKey;
        return externalLogin;
    }
}
