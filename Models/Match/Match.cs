namespace ServerStudy.Models;

public class Match
{
    public int Id { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
    public DateTime MatchedAt { get; set; }
}