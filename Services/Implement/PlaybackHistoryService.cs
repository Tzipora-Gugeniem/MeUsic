using Core.Modules;
using Core.Repository;
using Core.Services;

namespace Services;

public class PlaybackHistoryService : IPlaybackHistoryService
{
    private readonly IPlaybackHistoryRepository _historyRepository;

    public PlaybackHistoryService(IPlaybackHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task LogPlaybackAsync(int userId, int songId, double secondsPlayed, double totalDuration)
    {
        double listenPercentage = 0;

        // הגנה מפני חלוקה באפס (למקרה של שגיאה באורך השיר)
        if (totalDuration > 0)
        {
            // נוסחת החישוב לאחוז ההאזנה
            listenPercentage = (secondsPlayed / totalDuration) * 100;
            listenPercentage = Math.Round(listenPercentage, 2); // עיגול ל-2 ספרות אחרי הנקודה (למשל 84.35%)
        }

        var history = new PlaybackHistory
        {
            UserId = userId,
            SongId = songId,
            PlayedAt = DateTime.UtcNow
            // הערה: אם יש לך עמודה ייעודית לאחוזים במודל PlaybackHistory (למשל ListenPercentage),
            // תוכלי להוריד את הקומנט מהשורה הבאה:
            // ListenPercentage = listenPercentage
        };

        await _historyRepository.AddAsync(history);
        await _historyRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<PlaybackHistory>> GetUserHistoryAsync(int userId)
    {
        return await _historyRepository.GetByUserIdAsync(userId);
    }
}