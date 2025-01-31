using System.ComponentModel.DataAnnotations;

namespace ServerStudy.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequesterId { get; set; }
        public User Requester { get; set; }

        [Required]
        public int AddresseeId { get; set; }
        public User Addressee { get; set; }

        [Required]
        public FriendshipStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum FriendshipStatus
    {
        Pending,  // 대기 중
        Accepted, // 친구 수락됨
        Declined, // 요청 거절됨
        Blocked   // 차단됨
    }
}