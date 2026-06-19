namespace Core.Resources;

public class PlaybackHistoryResource
{
    public int Id { get; set; }
    public    int UserId { get; set; }
    public int SongId { get; set; }
    public DateTime PlayedAt { get; set; }
}
