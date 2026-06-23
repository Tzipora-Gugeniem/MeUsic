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
            PlayedAt = DateTime.UtcNow,
            // הערה: אם יש לך עמודה ייעודית לאחוזים במודל PlaybackHistory (למשל ListenPercentage),
            // תוכלי להוריד את הקומנט מהשורה הבאה:
             ListenPercentage = listenPercentage
        }; 

        await _historyRepository.AddAsync(history);
        await _historyRepository.SaveChangesAsync();
    }

    // משנים את ערך ההחזרה ל-PlaybackHistoryResource
    public async Task<IEnumerable<Core.Resources.PlaybackHistoryResource>> GetUserHistoryAsync(int userId)
    {
        var rawHistory = await _historyRepository.GetByUserIdAsync(userId);

        // הפיכת הנתונים הגולמיים לאובייקטים מעוצבים עם שמות
        return rawHistory.Select(ph => new Core.Resources.PlaybackHistoryResource
        {
            Id = ph.Id,
            Username = ph.User?.Username ?? "Unknown User", // שליפת שם המשתמש
            SongTitle = ph.Song?.Title ?? "Unknown Song",   // שליפת שם השיר
            Artist = ph.Song?.Artist ?? "Unknown Artist",   // שליפת שם האמן
            PlayedAt = ph.PlayedAt
        });
    }
}