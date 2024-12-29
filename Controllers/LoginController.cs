using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerStudy.DataBase;

namespace ServerStudy.Controllers;

public class LoginController : Controller
{
    private readonly UserData _userData;
    private readonly ILogger<LoginController> _logger;
    
    public LoginController(UserData userData, ILogger<LoginController> logger)
    {
        _userData = userData ?? throw new ArgumentNullException(nameof(userData));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _userData.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
        if (user == null)
        {
            _logger.LogError("Login failed for email: {Email}", email);
            return BadRequest(new { message = "Invalid email or password" });
        }
        
        _logger.LogInformation("Login successful for email: {Email}", email);
        
        HttpContext.Session.SetString("Useremail", user.Email);
        
        return Ok(new { message = "Login successful", userEmail = user.Email });
    }
}

