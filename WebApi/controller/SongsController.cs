using Microsoft.AspNetCore.Mvc;
using Core.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;
    private readonly IMichlolService _michlolService;

    public SongsController(ISongService songService, IMichlolService michlolService)
    {
        _songService = songService;
        _michlolService = michlolService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var songs = await _songService.GetAllSongsAsync();
        return Ok(songs);
    }

    [HttpGet("genre/{genre}")]
    public async Task<IActionResult> GetByGenre(string genre)
    {
        var songs = await _songService.GetSongsByGenreAsync(genre);
        return Ok(songs);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var songs = await _songService.SearchSongsAsync(query);
        return Ok(songs);
    }

    // פקודה להפעלת מנוע ההמלצות הלומד
    [HttpGet("recommendations/{userId}")]
    public async Task<IActionResult> GetRecommendations(int userId)
    {
        var recommendations = await _songService.GetRecommendedSongsAsync(userId);
        return Ok(recommendations);
    }

    // פקודה לשליפת ביוגרפיה מהמכלול לפי שם אמן
    [HttpGet("artist-bio/{artistName}")]
    public async Task<IActionResult> GetArtistBio(string artistName)
    {
        var bio = await _michlolService.GetArtistBioAsync(artistName);
        return Ok(new { artist = artistName, biography = bio });
    }
}