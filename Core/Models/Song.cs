namespace Core.Modules;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Artist { get; set; } = null!;
    public string Album { get; set; } = null!;
    public int Year { get; set; } 
    public string Genre { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public TimeSpan Duration { get; set; }
}
