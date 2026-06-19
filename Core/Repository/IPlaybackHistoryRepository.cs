using Core.Modules;

namespace Core.Repository;

public interface IPlaybackHistoryRepository
{
    Task AddAsync(PlaybackHistory history);
    Task<IEnumerable<PlaybackHistory>> GetByUserIdAsync(int userId);
    Task SaveChangesAsync();
}