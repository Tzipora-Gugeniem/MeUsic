using Core.Modules;

namespace Core.Services;

public interface ISongService
{
    Task<IEnumerable<Song>> GetAllSongsAsync();
    Task<IEnumerable<Song>> GetSongsByGenreAsync(string genre);
    Task<IEnumerable<Song>> SearchSongsAsync(string query);
    Task LogPlaybackAsync(int userId, int songId);

    //שירים מומלצים על סמך האזנות קודמות
    Task<IEnumerable<Song>> GetRecommendedSongsAsync(int userId);
}