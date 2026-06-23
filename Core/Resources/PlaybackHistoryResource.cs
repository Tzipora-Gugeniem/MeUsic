namespace Core.Resources;

public class PlaybackHistoryResource
{
    public int Id { get; set; }
    public string Username { get; set; }  // במקום UserId, נחזיר את שם המשתמש
    public string SongTitle { get; set; } // במקום SongId, נחזיר את שם השיר
    public string Artist { get; set; }    // נוסיף גם את שם האמן, שיהיה יפה!
    public DateTime PlayedAt { get; set; }
}