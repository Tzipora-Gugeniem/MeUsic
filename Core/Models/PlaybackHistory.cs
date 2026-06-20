namespace Core.Modules;

public class PlaybackHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SongId { get; set; }
    public Song? Song { get; set; }


    public DateTime PlayedAt { get; set; }
}
