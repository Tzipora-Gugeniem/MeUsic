using Microsoft.EntityFrameworkCore;
using Core.Modules;
using Core.Repository;

namespace DAL;

public class PlaybackHistoryRepository : IPlaybackHistoryRepository
{
    private readonly AppDbContext _context;

    public PlaybackHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlaybackHistory history)
    {
        await _context.PlaybackHistories.AddAsync(history);
    }

    public async Task<IEnumerable<PlaybackHistory>> GetByUserIdAsync(int userId)
    {
        return await _context.PlaybackHistories
            .Include(ph => ph.User) // טוען את נתוני המשתמש מהטבלה המקושרת
            .Include(ph => ph.Song) // טוען את נתוני השיר מהטבלה המקושרת
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.PlayedAt)
            .ToListAsync();
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}