namespace Core.Services;

public interface IMichlolService
{
    // פונקציה שמקבלת שם אמן ומחזירה את הביוגרפיה שלו כטקסט
    Task<string> GetArtistBioAsync(string artistName);
}
