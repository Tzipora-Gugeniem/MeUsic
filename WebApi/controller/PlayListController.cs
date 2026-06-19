using Microsoft.AspNetCore.Mvc;
using Core.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlistService;

    public PlaylistsController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromQuery] int userId, [FromQuery] string name)
    {
        var playlist = await _playlistService.CreatePlaylistAsync(userId, name);
        return Ok(playlist);
    }

    [HttpPost("add-song")]
    public async Task<IActionResult> AddSong([FromQuery] int playlistId, [FromQuery] int songId)
    {
        var success = await _playlistService.AddSongToPlaylistAsync(playlistId, songId);
        if (!success)
        {
            return BadRequest("השיר כבר קיים ברשימת ההשמעה הזו או שחלה שגיאה.");
        }
        return Ok(new { message = "השיר הוסף לפלייליסט בהצלחה." });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPlaylists(int userId)
    {
        var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
        return Ok(playlists);
    }
}