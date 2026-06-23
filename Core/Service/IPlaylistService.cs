using Core.Modules;

namespace Core.Services;

public interface IPlaylistService
{
    //יצירת רשימת השמעה חדשה
    Task<Playlist> CreatePlaylistAsync(int userId, string name);
    //הוסםת שיר לרשימת השמעה
    Task<bool> AddSongToPlaylistAsync(int playlistId, int songId);
    Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(int userId);
}