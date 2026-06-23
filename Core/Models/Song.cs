namespace Core.Modules;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = null!; // כותרת נשאיר כחובה (או שם קובץ)

    // הוספת סימן שאלה מאפשרת לערכים האלו להיות NULL במסד הנתונים!
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public int? Year { get; set; }        // שינוי ל-int? מאפשר שנה ריקה
    public string? Genre { get; set; }

    public string FilePath { get; set; } = null!;
    public TimeSpan Duration { get; set; }
}