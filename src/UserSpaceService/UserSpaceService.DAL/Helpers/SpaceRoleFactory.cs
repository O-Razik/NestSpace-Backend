using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Helpers;

public class SpaceRoleFactory : IEntityFactory<ISpaceRole>
{
    public ISpaceRole CreateEntity()
    {
        return new SpaceRole();
    }
}