using Core.Modules;
using System.Threading.Tasks;

namespace Core.Services;

public interface IPlaybackHistoryService
{
    // הפונקציה שנקראת כשהשיר כבה ומחשבת את אחוז ההאזנה
    Task LogPlaybackAsync(int userId, int songId, double secondsPlayed, double totalDuration);
    Task<IEnumerable<Core.Resources.PlaybackHistoryResource>> GetUserHistoryAsync(int userId);

    //Task LogPlaybackAsync(int userId, int songId);
}