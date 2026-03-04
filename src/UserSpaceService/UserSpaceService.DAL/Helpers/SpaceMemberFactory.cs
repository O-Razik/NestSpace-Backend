using UserSpaceService.ABS.IHelpers;
using UserSpaceService.ABS.IModels;
using UserSpaceService.DAL.Models;

namespace UserSpaceService.DAL.Helpers;

public class SpaceMemberFactory : IEntityFactory<ISpaceMember>
{
    public ISpaceMember CreateEntity()
    {
        return new SpaceMember();
    }
}
