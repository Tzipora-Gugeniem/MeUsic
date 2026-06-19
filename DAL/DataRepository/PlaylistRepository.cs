using Microsoft.EntityFrameworkCore;
using Core.Modules;
using Core.Repository;

namespace DAL;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly AppDbContext _context;

    public PlaylistRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Playlist> CreateAsync(Playlist playlist)
    {
        await _context.Playlists.AddAsync(playlist);
        return playlist;
    }

    public async Task<bool> IsSongInPlaylistAsync(int playlistId, int songId)
    {
        return await _context.PlaylistSongs
            .AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
    }

    public async Task AddSongToPlaylistAsync(PlaylistSong playlistSong)
    {
        await _context.PlaylistSongs.AddAsync(playlistSong);
    }

    public async Task<IEnumerable<Playlist>> GetByUserIdAsync(int userId)
    {
        return await _context.Playlists
            .Where(p => p.UserId == userId)
            .Include(p => p.PlaylistSongs)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}