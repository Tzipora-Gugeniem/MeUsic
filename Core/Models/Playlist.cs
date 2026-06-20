namespace Core.Modules;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int UserId { get; set; }
    public User? User { get; set; }

    // Navigation to PlaylistSong join
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}
