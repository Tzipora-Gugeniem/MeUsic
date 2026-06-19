using Microsoft.AspNetCore.Mvc;
using Core.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaybackHistoryController : ControllerBase
{
    private readonly IPlaybackHistoryService _historyService;

    public PlaybackHistoryController(IPlaybackHistoryService historyService)
    {
        _historyService = historyService;
    }

    // יצירת רשומת האזנה חדשה (חישוב אחוזים בעת כיבוי/עצירת השיר)
    [HttpPost("log")]
    public async Task<IActionResult> LogPlayback([FromQuery] int userId, [FromQuery] int songId, [FromQuery] double secondsPlayed, [FromQuery] double totalDuration)
    {
        await _historyService.LogPlaybackAsync(userId, songId, secondsPlayed, totalDuration);
        return Ok(new { message = "היסטוריית ההאזנה עודכנה וחוברו אחוזי הנגינה בהצלחה." });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserHistory(int userId)
    {
        var history = await _historyService.GetUserHistoryAsync(userId);
        return Ok(history);
    }
}