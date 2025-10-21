using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Helpers;

public class UserFactory : IEntityFactory<IUser>
{
    public IUser CreateEntity()
    {
        return new User();
    }
}