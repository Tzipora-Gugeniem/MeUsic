namespace Core.Resources;

public class SongResource
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Artist { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public TimeSpan Duration { get; set; }
}
