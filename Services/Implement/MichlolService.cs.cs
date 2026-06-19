using System.Net.Http;
using System.Text.Json;
using Core.Services;

namespace Services;

public class MichlolService : IMichlolService
{
    private readonly HttpClient _httpClient;

    // הזרקת ה-HttpClient לקבלת יכולות גישה לרשת
    public MichlolService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    //קריאת API חיצוני לויקיפדיה(נטפרי מפנה למכלול)לקבלת מידע על האמן הנבחר
    public async Task<string> GetArtistBioAsync(string artistName)
    {
        if (string.IsNullOrWhiteSpace(artistName))
            return "לא צוין שם אמן.";

        try
        {
            // בניית הכתובת הדינמית עם הפרמטרים של MediaWiki והמכלול
            string url = $"https://www.hamichlol.org.il/w/api.php?action=query&prop=extracts&exintro&explaintext&format=json&titles={Uri.EscapeDataString(artistName)}";

            // שליחת בקשת ה-GET בפועל לשרת של המכלול
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "שגיאה: לא ניתן היה לגשת לשרתי המכלול.";

            // קריאת תוכן התשובה כטקסט (JSON)
            string jsonString = await response.Content.ReadAsStringAsync();

            // ניתוח ה-JSON ומציאת שדה ה-extract המכיל את הביוגרפיה
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            var pages = doc.RootElement.GetProperty("query").GetProperty("pages");

            foreach (var page in pages.EnumerateObject())
            {
                // אם הערך קיים, ה-API מחזיר אובייקט עם שדה בשם extract
                if (page.Value.TryGetProperty("extract", out var extractElement))
                {
                    string bio = extractElement.GetString() ?? "";
                    return !string.IsNullOrWhiteSpace(bio) ? bio : "לא נמצא מידע מורחב על אמן זה.";
                }
            }

            return "הערך אינו קיים באתר המכלול.";
        }
        catch (Exception ex)
        {
            return $"שגיאה בלתי צפויה בעת שליפת הנתונים: {ex.Message}";
        }
    }
}
