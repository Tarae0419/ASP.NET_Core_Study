namespace ServerStudy.Models;

public class Player
{
    public int Id { get; set; }
    public DateTime EnqueuedAt { get; set; }
    public bool IsMatched { get; set; }
}