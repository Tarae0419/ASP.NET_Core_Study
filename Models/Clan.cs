namespace ServerStudy.Models;

public class Clan
{
    public int ClanId { get; set; }
    
    public string ClanName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public ICollection<User> Users { get; set; }
    public int LeaderId { get; set; }
    public User Leader { get; set; }
}