using Microsoft.AspNetCore.Mvc;
using ServerStudy.DataBase;
using ServerStudy.Models;
using Microsoft.AspNetCore.Identity;

namespace ServerStudy.Controllers;

[Route("api/register")]
[ApiController]
public class CreateAccountController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<CreateAccountController> _logger;
    private readonly PasswordHasher<User> _passwordHasher;
    
    public CreateAccountController(ApplicationDbContext applicationDbContext, ILogger<CreateAccountController> logger)
    {
        _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordHasher = new PasswordHasher<User>();
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest(new { message = "유저 정보 필요" });
        }

        if (_applicationDbContext.Users.Any(u => u.Email == user.Email))
        {
            return Conflict(new { message = "이메일 중복" });
        }
        
        if (_applicationDbContext.Users.Any(u => u.Nickname == user.Nickname))
        {
            return Conflict(new { message = "닉네임 중복" });
        }
        
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        
        _applicationDbContext.Users.Add(user);
        await _applicationDbContext.SaveChangesAsync();
        
        _logger.LogInformation("New user registered: {Username}, Nickname: {Nickname}", user.Email, user.Nickname);
            
        return Ok(new { message = "User successfully registered", email = user.Email, nickname = user.Nickname });
    }
}