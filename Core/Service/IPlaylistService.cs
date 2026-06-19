using Core.Modules;

namespace Core.Services;

public interface IPlaylistService
{
    Task<Playlist> CreatePlaylistAsync(int userId, string name);
    Task<bool> AddSongToPlaylistAsync(int playlistId, int songId);
    Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(int userId);
}