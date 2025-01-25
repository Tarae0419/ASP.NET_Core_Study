namespace ServerStudy.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Nicname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ClanName { get; set; }
}