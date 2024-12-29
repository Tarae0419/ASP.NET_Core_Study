using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ServerStudy.DataBase;
using ServerStudy.Models;

namespace ServerStudy.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly UserData _userData;
    private readonly ILogger<LoginController> _logger;
    private readonly PasswordHasher<User> _passwordHasher;

    public LoginController(UserData userData, ILogger<LoginController> logger)
    {
        _userData = userData ?? throw new ArgumentNullException(nameof(userData));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordHasher = new PasswordHasher<User>();
    }
    
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _userData.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        if (user == null)
        {
            _logger.LogError("로그인 실패 : {Email}", email);
            return BadRequest(new { message = "아이디 또는 비밀번호가 맞지 않음" });
        }
        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "아이디 또는 비밀번호가 맞지 않음" });
        }
        
        _logger.LogInformation("로그인 성공: {Email}", email);
        
        HttpContext.Session.SetString("Useremail", user.Email);
        
        return Ok(new { message = "로그인 성공", userEmail = user.Email });
    }
}

