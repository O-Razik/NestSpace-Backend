using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.BLL.Dtos.Send;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatNotifyService.API.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="activityLogService"></param>
/// <param name="httpContextAccessor"></param>
/// <param name="activityLogMapper"></param>
[Authorize]
[Route("api/space/{spaceId:guid}/activity-logs")]
[ApiController]
public class SpaceActivityLogController(
    ISpaceActivityLogService activityLogService,
    IHttpContextAccessor httpContextAccessor,
    IMapper<ISpaceActivityLog, SpaceActivityLogDto> activityLogMapper) 
    : ControllerBase
{
    /// <summary>
    /// Gets activity logs for a specific space with pagination.
    /// </summary>
    /// <param name="spaceId"></param>
    /// <param name="page"></param>
    /// <param name="amount"></param>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpaceActivityLogDto>>> GetActivityLogs(
        [FromRoute] Guid spaceId,
        [FromQuery] int page = 1,
        [FromQuery] int amount = 20)
    {
        var logs = await activityLogService.GetActivityLogsBySpaceAsync(spaceId, page, amount);
        return Ok(logs.Select(activityLogMapper.ToDto));
    }
}