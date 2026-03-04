using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Helpers;

public class SpaceFactory : IEntityFactory<ISpace>
{
    public ISpace CreateEntity()
    {
        return new Space();
    }
}
