using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;
using ServerStudy.Models;

namespace ServerStudy.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ChatController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // 메시지 보내기
    [HttpPost("{roomName}/send")]
    public async Task<IActionResult> SendMessage(string roomName, [FromBody] Message message)
    {
        if (string.IsNullOrEmpty(message.Sender) || string.IsNullOrEmpty(message.Content))
        {
            return BadRequest(new { success = false, error = "발신자 또는 메세지 내용이 필요" });
        }

        //채팅방 조회 및 생성
        var chatRoom = await _dbContext.ChatRooms
            .Include(r => r.Messages)
            .FirstOrDefaultAsync(r => r.RoomName == roomName);

        if (chatRoom == null)
        {
            chatRoom = new ChatRoom { RoomName = roomName };
            _dbContext.ChatRooms.Add(chatRoom);
        }
        
        // 메시지 추가
        message.Timestamp = DateTime.UtcNow;
        chatRoom.Messages.Add(message);
        
        await _dbContext.SaveChangesAsync();

        return Ok(new { success = true, message = "메시지 보내기 성공" });
    }

    //메시지 가져오기
    [HttpGet("{roomName}/messages")]
    public async Task<IActionResult> GetMessages(string roomName, [FromQuery] DateTime? since = null)
    {
        var chatRoom = await _dbContext.ChatRooms
            .Include(r => r.Messages)
            .FirstOrDefaultAsync(r => r.RoomName == roomName);
        
        
        if (chatRoom == null)
        {
            return NotFound(new { success = false, error = "채팅방을 찾을 수 없음" });
        }

        var messages = since.HasValue
            ? chatRoom.Messages.Where(m => m.Timestamp > since.Value).ToList()
            : chatRoom.Messages;
        
        return Ok(messages);
    }

}