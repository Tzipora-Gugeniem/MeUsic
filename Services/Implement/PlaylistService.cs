using Core.Modules;
using Core.Repository;
using Core.Services;

namespace Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepository;

    public PlaylistService(IPlaylistRepository playlistRepository)
    {
        _playlistRepository = playlistRepository;
    }

    public async Task<Playlist> CreatePlaylistAsync(int userId, string name)
    {
        var playlist = new Playlist
        {
            UserId = userId,
            Name = name,
            //CreatedAt = DateTime.UtcNow
        };

        var result = await _playlistRepository.CreateAsync(playlist);
        await _playlistRepository.SaveChangesAsync();
        return result;
    }

    public async Task<bool> AddSongToPlaylistAsync(int playlistId, int songId)
    {
        var exists = await _playlistRepository.IsSongInPlaylistAsync(playlistId, songId);
        if (exists) return false;

        var playlistSong = new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId
        };

        await _playlistRepository.AddSongToPlaylistAsync(playlistSong);
        await _playlistRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(int userId)
    {
        return await _playlistRepository.GetByUserIdAsync(userId);
    }
}