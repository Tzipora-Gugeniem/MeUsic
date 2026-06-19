using Core.Modules;

namespace Core.Repository;

public interface ISongRepository
{
    Task<IEnumerable<Song>> GetAllAsync();
    Task<IEnumerable<Song>> GetByGenreAsync(string genre);
    Task<IEnumerable<Song>> SearchAsync(string query);
    Task AddPlaybackHistoryAsync(PlaybackHistory history);
    Task SaveChangesAsync();
}