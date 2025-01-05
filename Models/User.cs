namespace ServerStudy.Models;

public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Nickname { get; set; }
    public int ClanId { get; set; }
    public Clan Clan { get; set; }
}