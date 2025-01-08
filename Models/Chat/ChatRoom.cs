namespace ServerStudy.Models;

public class ChatRoom
{
    public int Id { get; set; }
    public string RoomName { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
}