using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Helpers;

public class ExternalLoginFactory : IEntityFactory<IExternalLogin>
{
    public IExternalLogin CreateEntity()
    {
        return new ExternalLogin();
    }
}
