using Core.Modules;

namespace Core.Repository;

public interface IPlaylistRepository
{
    Task<Playlist> CreateAsync(Playlist playlist);
    Task<bool> IsSongInPlaylistAsync(int playlistId, int songId);
    Task AddSongToPlaylistAsync(PlaylistSong playlistSong);
    Task<IEnumerable<Playlist>> GetByUserIdAsync(int userId);
    Task SaveChangesAsync();
}