using Core.Modules;

namespace Core.Services;

public interface ISongService
{
    Task<IEnumerable<Song>> GetAllSongsAsync();
    Task<IEnumerable<Song>> GetSongsByGenreAsync(string genre);
    Task<IEnumerable<Song>> SearchSongsAsync(string query);
    //הכנסת השירים הקיימים במסד לתוך האפליקציה
    Task<int> ScanMusicFolderAsync(string folderPath = @"C:\MusicFolder");

    //שירים מומלצים על סמך האזנות קודמות
    Task<IEnumerable<Song>> GetRecommendedSongsAsync(int userId);
}