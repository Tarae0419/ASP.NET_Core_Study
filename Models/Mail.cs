using System.ComponentModel.DataAnnotations;

namespace ServerStudy.Models;

public class Mail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ReceiverId { get; set; }
    public User Receiver { get; set; } 

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string ItemName { get; set; }
    public int? Quantity { get; set; }

    [Required]
    public bool IsRead { get; set; } = false;

    [Required]
    public bool IsCollected { get; set; } = false;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
