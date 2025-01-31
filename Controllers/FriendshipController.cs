using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;
using ServerStudy.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ServerStudy.Controllers
{
    [ApiController]
    [Route("api/friends")]
    public class FriendshipController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public FriendshipController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        // 1. 친구 요청 보내기
        [HttpPost("request/{requesterId}/{addresseeId}")]
        public async Task<IActionResult> SendFriendRequest(int requesterId, int addresseeId)
        {
            if (requesterId == addresseeId)
                return BadRequest(new { success = false, message = "자신 불가" });

            var existingFriendship = await _dbContext.Friendships
                .FirstOrDefaultAsync(f => (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                                          (f.RequesterId == addresseeId && f.AddresseeId == requesterId));

            if (existingFriendship != null)
                return BadRequest(new { success = false, message = "이미 존재하거나 친구 상태" });

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                Status = FriendshipStatus.Pending
            };

            _dbContext.Friendships.Add(friendship);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "친구 요청 성공" });
        }
        
        // 2. 친구 요청 수락 또는 거절
        [HttpPost("response/{friendshipId}/{isAccepted}")]
        public async Task<IActionResult> RespondToFriendRequest(int friendshipId, bool isAccepted)
        {
            var friendship = await _dbContext.Friendships.FindAsync(friendshipId);
            if (friendship == null)
                return NotFound(new { success = false, message = "친구 요청을 찾을 수 없습니다." });

            friendship.Status = isAccepted ? FriendshipStatus.Accepted : FriendshipStatus.Declined;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = isAccepted ? "친구 요청 수락" : "친구 요청 거절" });
        }
        
        // 3. 친구 목록 조회
        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriendList(int userId)
        {
            var friends = await _dbContext.Friendships
                .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) && f.Status == FriendshipStatus.Accepted)
                .Select(f => new
                {
                    FriendId = f.RequesterId == userId ? f.AddresseeId : f.RequesterId
                })
                .ToListAsync();

            return Ok(new { success = true, friends });
        }
        
        // 4. 친구 삭제
        [HttpDelete("remove/{friendshipId}")]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            var friendship = await _dbContext.Friendships.FindAsync(friendshipId);
            if (friendship == null)
                return NotFound(new { success = false, message = "친구 관계를 찾을 수 없습니다." });

            _dbContext.Friendships.Remove(friendship);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "친구가 삭제되었습니다." });
        }
        
        // 5. 친구 차단
        [HttpPost("block/{friendshipId}")]
        public async Task<IActionResult> BlockFriend(int friendshipId)
        {
            var friendship = await _dbContext.Friendships.FindAsync(friendshipId);
            if (friendship == null)
                return NotFound(new { success = false, message = "친구 관계를 찾을 수 없습니다." });

            friendship.Status = FriendshipStatus.Blocked;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "사용자가 차단되었습니다." });
        }
    }
}
