//using Core.Repository;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
// תצטרכי לוודא שיש לך using לפרופיל המיפוי שלך, למשל:
// using Core.Mapping; 

var builder = WebApplication.CreateBuilder(args);

// 1. הגדרת CORS ( מאפשר גישה לפרונטאנד)
builder.Services.AddCors(options => options.AddPolicy("allowAny", o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// 2. הזרקה ישירה של מחרוזת החיבור למסד הנתונים
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MeUsicDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// --- רישום שכבות ה-Repository וה-Services שלך ---
// 1. היסטוריית האזנות
builder.Services.AddScoped<Core.Repository.IPlaybackHistoryRepository, DAL.PlaybackHistoryRepository>();
builder.Services.AddScoped<Core.Services.IPlaybackHistoryService, Services.PlaybackHistoryService>();

// 2. שירים ומנוע המלצות
builder.Services.AddScoped<Core.Repository.ISongRepository, DAL.SongRepository>();
builder.Services.AddScoped<Core.Services.ISongService, Services.SongService>();

// 3. פלייליסטים
builder.Services.AddScoped<Core.Repository.IPlaylistRepository, DAL.PlaylistRepository>();
builder.Services.AddScoped<Core.Services.IPlaylistService, Services.PlaylistService>();

builder.Services.AddScoped<Core.Repository.IUserRepository, DAL.UserRepository>();
builder.Services.AddScoped<Core.Services.IUserService, Services.UserService>();
// 5. ה-HttpClient של המכלול
builder.Services.AddHttpClient<Core.Services.IMichlolService, Services.MichlolService>();

// 4. הגדרת Swagger משופרת (כמו אצל המורה)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeUsic.Api", Version = "v1" });
});

var app = builder.Build();

// הגדרת צינור הבקשות (Pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeUsic.Api V1");
    });
}

app.UseHttpsRedirection();

// הפעלת מדיניות ה-CORS שרשמנו למעלה
app.UseCors("allowAny");

app.UseAuthorization();
app.MapControllers();

app.Run();