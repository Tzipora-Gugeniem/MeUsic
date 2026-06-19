using Core.Modules;

namespace Core.Services;

public interface IPlaybackHistoryService
{
    // הפונקציה שנקראת כשהשיר כבה ומחשבת את אחוז ההאזנה
    Task LogPlaybackAsync(int userId, int songId, double secondsPlayed, double totalDuration);
    Task<IEnumerable<PlaybackHistory>> GetUserHistoryAsync(int userId);
}