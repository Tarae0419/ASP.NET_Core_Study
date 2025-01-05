using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ServerStudy.DataBase;
using ServerStudy.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerStudy.DTOs;

namespace ServerStudy.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<LoginController> _logger;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public LoginController(ApplicationDbContext applicationDbContext, ILogger<LoginController> logger, IConfiguration configuration)
    {
        _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordHasher = new PasswordHasher<User>();
        _configuration = configuration;
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto )
    {
        var user = await _applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        
        if (user == null)
        {
            _logger.LogWarning("로그인 실패 : 아이디가 존재하지 않습니다. {Email}", loginDto.Email);
            return BadRequest(new { message = "아이디 또는 비밀번호가 맞지 않음" });
        }
        
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("로그인 실패: 잘못된 비밀번호 {Email}", loginDto.Email);
            return Unauthorized(new { message = "아이디 또는 비밀번호가 맞지 않음" });
        }
        
        var token = GenerateJwtToken(user);
        
        _logger.LogInformation("로그인 성공: {Email}", loginDto.Email);
        
        return Ok(new { Token = token });
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim (JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

