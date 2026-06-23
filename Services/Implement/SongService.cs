using Core.Modules;
using Core.Repository;
using Core.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using TagLib; 


namespace Services;

public class SongService : ISongService
{
    private readonly ISongRepository _songRepository;
  
    private readonly IPlaybackHistoryRepository _historyRepository; // הזרקה נוספת לצורך למידה

    public SongService(ISongRepository songRepository, IPlaybackHistoryRepository historyRepository)
    {
        _songRepository = songRepository;
        _historyRepository = historyRepository;
    }
    // הזרקת הממשק מתוך Core! ניתוק מלא מה-DbContext האמיתי
    public SongService(ISongRepository songRepository)
    {
        _songRepository = songRepository;
    }
    public async Task<int> ScanMusicFolderAsync(string folderPath = @"C:\MusicFolder")
    {
        // 1. בדיקה שהתיקייה הזו בכלל קיימת במחשב
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException("התיקייה המבוקשת לא נמצאה במחשב.");
        }

        // 2. שליפת כל קבצי ה-mp3 מתוך התיקייה במחשב
        var mp3Files = Directory.GetFiles(folderPath, "*.mp3", SearchOption.AllDirectories);
        int songsAdded = 0;

        // --- שיפור הביצועים הקריטי: פנייה אחת ויחידה למסד הנתונים! ---
        // אנחנו שולפים את כל השירים הקיימים, ומחזיקים רק את ה-FilePath שלהם בתוך HashSet
        var allSongsInDb = await _songRepository.GetAllAsync();
        var existingPaths = new HashSet<string>(allSongsInDb.Select(s => s.FilePath), StringComparer.OrdinalIgnoreCase);
        // -------------------------------------------------------------

        foreach (var filePath in mp3Files)
        {
            // בדיקה בזיכרון המקומי של המחשב (לוקח 0 מילישניות ולא פונה ל-SQL!)
            if (existingPaths.Contains(filePath))
            {
                Console.WriteLine($"[Skip] השיר כבר קיים במסד הנתונים: {filePath}");
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // הגדרת ערכי המקור כ-null (פרט לכותרת, אותה נגבה בשם הקובץ)
            string songTitle = fileName;
            string? artistName = null;
            string? albumName = null;
            int? songYear = null;
            string? genreName = null;
            TimeSpan duration = TimeSpan.FromMinutes(3);

            try
            {
                using (var tfile = TagLib.File.Create(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(tfile.Tag.Title))
                    {
                        songTitle = tfile.Tag.Title;
                    }

                    if (tfile.Tag.Performers != null && tfile.Tag.Performers.Length > 0)
                    {
                        var validPerformers = tfile.Tag.Performers.Where(p => !string.IsNullOrWhiteSpace(p));
                        if (validPerformers.Any())
                        {
                            artistName = string.Join(", ", validPerformers);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(tfile.Tag.Album))
                    {
                        albumName = tfile.Tag.Album;
                    }

                    if (tfile.Tag.Year > 0)
                    {
                        songYear = (int)tfile.Tag.Year;
                    }

                    if (tfile.Tag.Genres != null && tfile.Tag.Genres.Length > 0)
                    {
                        var validGenres = tfile.Tag.Genres.Where(g => !string.IsNullOrWhiteSpace(g));
                        if (validGenres.Any())
                        {
                            genreName = string.Join(", ", validGenres);
                        }
                    }

                    if (tfile.Properties.Duration.TotalSeconds > 0)
                    {
                        duration = tfile.Properties.Duration;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] כשל בקריאת קובץ: {filePath}. שגיאה: {ex.Message}");
            }

            // יצירת האובייקט
            var newSong = new Song
            {
                Title = songTitle,
                Artist = artistName,
                Album = albumName,
                Year = songYear,
                Genre = genreName,
                FilePath = filePath,
                Duration = duration
            };

            // שליחת הישות אל הרפוזיטורי (נשמר בזיכרון הזמני)
            await _songRepository.AddAsync(newSong);
            songsAdded++;
        }

        // שמירה מרוכזת אחת קדימה לתוך ה-SQL Server עבור כל השירים החדשים
        await _songRepository.SaveChangesAsync();

        return songsAdded;
    }

    public async Task<IEnumerable<Song>> GetAllSongsAsync()
    {
        return await _songRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Song>> GetSongsByGenreAsync(string genre)
    {
        return await _songRepository.GetByGenreAsync(genre);
    }

    public async Task<IEnumerable<Song>> SearchSongsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<Song>();

        return await _songRepository.SearchAsync(query);
    }

    
    //פונקציה חכמה למציאת המוזיקה המועדפת
    public async Task<IEnumerable<Song>> GetRecommendedSongsAsync(int userId)
    {
        // 1. שליפת היסטוריית ההאזנות של המשתמש
        var history = await _historyRepository.GetByUserIdAsync(userId);

        // אם המשתמש חדש, נחזיר 5 שירים כלליים
        if (history == null || !history.Any())
        {
            var allSongs = await _songRepository.GetAllAsync();
            return allSongs.Take(5);
        }

        var validHistory = history.Where(h => h.Song != null).ToList();
        if (!validHistory.Any())
        {
            var allSongs = await _songRepository.GetAllAsync();
            return allSongs.Take(5);
        }

        // 2. מציאת המאפיין הדומיננטי ביותר מכל השדות של השיר

        // קבוצת האמנים המובילה והכמות שלה
        var topArtistGroup = validHistory.GroupBy(h => h.Song.Artist).OrderByDescending(g => g.Count()).FirstOrDefault();
        // קבוצת הז'אנרים המובילה והכמות שלה
        var topGenreGroup = validHistory.GroupBy(h => h.Song.Genre).OrderByDescending(g => g.Count()).FirstOrDefault();
        // קבוצת האלבומים המובילה והכמות שלה
        var topAlbumGroup = validHistory.Where(h => !string.IsNullOrEmpty(h.Song.Album)).GroupBy(h => h.Song.Album).OrderByDescending(g => g.Count()).FirstOrDefault();
        // קבוצת השנים המובילה והכמות שלה
        var topYearGroup = validHistory.GroupBy(h => h.Song.Year).OrderByDescending(g => g.Count()).FirstOrDefault();

        // חילוץ כמויות (אם הקבוצה ריקה, נשים 0)
        int artistCount = topArtistGroup?.Count() ?? 0;
        int genreCount = topGenreGroup?.Count() ?? 0;
        int albumCount = topAlbumGroup?.Count() ?? 0;
        int yearCount = topYearGroup?.Count() ?? 0;

        // 3. מציאת ה"מנצח" - מה המאפיין שחזר על עצמו הכי הרבה פעמים?
        int maxCount = Math.Max(artistCount, Math.Max(genreCount, Math.Max(albumCount, yearCount)));

        // שליפת כל השירים הקיימים כדי לבצע מתוכם את הסינון וההמלצה
        var allAvailableSongs = await _songRepository.GetAllAsync();
        var listenedSongIds = validHistory.Select(h => h.SongId).Distinct().ToList();

        IEnumerable<Song> filteredSongs = Enumerable.Empty<Song>();

        // 4. שליפת שירים חדשים לפי המאפיין הניצח
        if (maxCount == artistCount && topArtistGroup != null)
        {
            // המשתמש נאמן לזמר מסוים! נמליץ על שירים נוספים של אותו זמר
            filteredSongs = allAvailableSongs.Where(s => s.Artist == topArtistGroup.Key);
        }
        else if (maxCount == genreCount && topGenreGroup != null)
        {
            // המשתמש אוהב סגנון מסוים! נמליץ על שירים מאותו ז'אנר
            filteredSongs = allAvailableSongs.Where(s => s.Genre == topGenreGroup.Key);
        }
        else if (maxCount == albumCount && topAlbumGroup != null)
        {
            // המשתמש חורש על אלבום ספציפי! נמליץ על שירים נוספים מאותו אלבום
            filteredSongs = allAvailableSongs.Where(s => s.Album == topAlbumGroup.Key);
        }
        else if (maxCount == yearCount && topYearGroup != null)
        {
            // המשתמש אוהב תקופה מסוימת (למשל שנות ה-90)! נמליץ על שירים מאותה שנה
            filteredSongs = allAvailableSongs.Where(s => s.Year == topYearGroup.Key);
        }

        // 5. סינון שירים שהמשתמש כבר שמע ולקיחת 5 המלצות
        var recommendations = filteredSongs
            .Where(s => !listenedSongIds.Contains(s.Id))
            .Take(5)
            .ToList();

        // 6. רשת ביטחון: אם המשתמש כבר שמע את כל השירים של המאפיין המנצח, 
        // נחזיר לו שירים מהמאפיין הבא בתור (ז'אנר כברירת מחדל רחבה)
        if (!recommendations.Any() && topGenreGroup != null)
        {
            recommendations = allAvailableSongs
                .Where(s => s.Genre == topGenreGroup.Key && !listenedSongIds.Contains(s.Id))
                .Take(5)
                .ToList();
        }

        // מוצא אחרון בהחלט: שירים כלליים שלא נשמעו
        return recommendations.Any() ? recommendations : allAvailableSongs.Where(s => !listenedSongIds.Contains(s.Id)).Take(5);
    }

}