using Core.Modules;

namespace Core.Services;

public interface IUserService
{
    Task<User?> RegisterAsync(string username, string email, string password);
    Task<string?> LoginAsync(string email, string password); // מחזיר טוקן או מזהה הצלחה
    Task<User?> GetUserByIdAsync(int id);
}