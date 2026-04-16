using ChatNotifyService.ABS.Dtos;
using ChatNotifyService.ABS.IHelpers;
using ChatNotifyService.ABS.IServices;
using ChatNotifyService.ABS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Guard = ChatNotifyService.BLL.Helpers.Guard;

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
    IMapper<SpaceActivityLog, SpaceActivityLogDto> activityLogMapper) 
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
        [FromQuery] int page,
        [FromQuery] int amount)
    {
        Guard.AgainstEmptyGuid(spaceId);
        Guard.AgainstNegativeOrZero(page);
        Guard.AgainstNegativeOrZero(amount);
        var logs = await activityLogService.GetActivityLogsBySpaceAsync(spaceId, page, amount);
        return Ok(logs.Select(activityLogMapper.ToDto));
    }
}
