namespace ServerStudy.Models;

public class ClanDto
{
    public int Id { get; set; }
    public string ClanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string>? MemberNicknames { get; set; }
}