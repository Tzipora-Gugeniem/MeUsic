using Core.Repository;
using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// הזרקה ישירה של מחרוזת החיבור
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MeUsicDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"));

// ודאי שורה זו קיימת - היא רושמת את כל ה-Controllers במערכת!
builder.Services.AddControllers();

// הגדרות Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- רישום כל שכבות ה-Repository וה-Services במערכת ---

// 1. היסטוריית האזנות
builder.Services.AddScoped<Core.Repository.IPlaybackHistoryRepository, DAL.PlaybackHistoryRepository>();
builder.Services.AddScoped<Core.Services.IPlaybackHistoryService, Services.PlaybackHistoryService>();

// 2. שירים ומנוע המלצות
builder.Services.AddScoped<Core.Repository.ISongRepository, DAL.SongRepository>();
builder.Services.AddScoped<Core.Services.ISongService, Services.SongService>();

// 3. פלייליסטים
builder.Services.AddScoped<Core.Repository.IPlaylistRepository, DAL.PlaylistRepository>();
builder.Services.AddScoped<Core.Services.IPlaylistService, Services.PlaylistService>();

// 4. משתמשים

builder.Services.AddScoped<Core.Repository.IUserRepository, DAL.UserRepository>();
builder.Services.AddScoped<Core.Services.IUserService, Services.UserService>();

// 5. ה-HttpClient של המכלול
builder.Services.AddHttpClient<Core.Services.IMichlolService, Services.MichlolService>();

var app = builder.Build();

// הגדרת צינור הבקשות (Pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// הוספת ניתוב משתמשים (חשוב עבור קונטרולרים)
app.UseAuthorization();

// שורה קריטית: מחברת את נתיבי ה-Controllers ל-Swagger ולרשת
app.MapControllers();

// פונקציית ברירת המחדל של מזג האוויר (ניתן להשאיר או למחוק)
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}