using Microsoft.EntityFrameworkCore;
using Core.Modules;
using Core.Repository;

namespace DAL;

public class SongRepository : ISongRepository
{
    private readonly AppDbContext _context;

    // הזרקת ה-DbContext לפעולות ה-CRUD הבסיסיות
    public SongRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Song>> GetAllAsync()
    {
        return await _context.Songs.ToListAsync();
    }

    public async Task<IEnumerable<Song>> GetByGenreAsync(string genre)
    {
        return await _context.Songs
            .Where(s => s.Genre.ToLower() == genre.ToLower())
            .ToListAsync();
    }

    public async Task<IEnumerable<Song>> SearchAsync(string query)
    {
        return await _context.Songs
            .Where(s => s.Title.Contains(query) || s.Artist.Contains(query))
            .ToListAsync();
    }

  public async Task AddAsync(Song song)
    {
        await _context.Songs.AddAsync(song);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}