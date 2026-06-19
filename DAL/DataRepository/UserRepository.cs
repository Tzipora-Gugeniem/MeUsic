using Core.Modules;
using Core.Repository;
using Core.Services;

namespace DAL;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> RegisterAsync(string username, string email, string password)
    {
        // לוגיקה עסקית 1: בדיקה האם המייל כבר תפוס במערכת
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            return null; // המשתמש כבר קיים, הרישום נכשל
        }

        // יצירת הישות החדשה
        var user = new User
        {
            Username = username,
            Email = email,
         Password = password
        };

        // שמירה במסד הנתונים דרך הרפוסיטורי
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        // לוגיקה עסקית 2: שליפת המשתמש לפי המייל שלו
        var user = await _userRepository.GetByEmailAsync(email);

        // בדיקה האם המשתמש קיים והאם הסיסמה תואמת
        if (user == null || user.Password != password)
        {
            return null; // אימות נכשל
        }

        // החזרת טוקן זמני (בהמשך נחליף לטוקן JWT אמיתי ומאובטח)
        return $"mock-jwt-token-for-user-{user.Id}";
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}