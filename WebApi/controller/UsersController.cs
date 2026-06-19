using Microsoft.AspNetCore.Mvc;
using Core.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string email, [FromQuery] string password)
    {
        var user = await _userService.RegisterAsync(username, email, password);
        if (user == null)
        {
            return BadRequest("הרישום נכשל. ייתכן וכתובת המייל הזו כבר קיימת במערכת.");
        }
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromQuery] string email, [FromQuery] string password)
    {
        var token = await _userService.LoginAsync(email, password);
        if (token == null)
        {
            return Unauthorized("שם המשתמש או הסיסמה שגויים.");
        }
        return Ok(new { token = token, message = "התחברת בהצלחה!" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound("המשתמש לא נמצא.");
        return Ok(user);
    }
}