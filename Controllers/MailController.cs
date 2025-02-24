using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;
using ServerStudy.Models;

namespace ServerStudy.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public MailController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        // 우편 발송
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromBody] Mail mail)
        {
            var receiver = await _dbContext.Users.FindAsync(mail.ReceiverId);
            if (receiver == null)
                return NotFound(new { success = false, message = "받는 유저를 찾을 수 없습니다." });

            _dbContext.Mails.Add(mail);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "우편이 성공적으로 보내졌습니다." });
        }
        
        // 우편 조회
        [HttpGet("inbox/{receiverId}")]
        public async Task<IActionResult> GetInbox(int receiverId)
        {
            var mails = await _dbContext.Mails
                .Where(m => m.ReceiverId == receiverId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            return Ok(new { success = true, mails });
        }
        
        // 우편 읽기
        [HttpPost("read/{mailId}")]
        public async Task<IActionResult> ReadMail(int mailId)
        {
            var mail = await _dbContext.Mails.FindAsync(mailId);
            if (mail == null)
                return NotFound(new { success = false, message = "우편을 찾을 수 없습니다." });

            mail.IsRead = true;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "우편이 읽음 처리되었습니다." });
        }
        
        // 우편 수령
        [HttpPost("collect/{mailId}")]
        public async Task<IActionResult> CollectReward(int mailId)
        {
            var mail = await _dbContext.Mails.FindAsync(mailId);
            if (mail == null)
                return NotFound(new { success = false, message = "우편을 찾을 수 없습니다." });

            if (mail.IsCollected)
                return BadRequest(new { success = false, message = "이미 수령한 보상입니다." });

            if (string.IsNullOrEmpty(mail.ItemName) || mail.Quantity == null)
                return BadRequest(new { success = false, message = "이 우편에는 보상이 없습니다." });

            mail.IsCollected = true;
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "보상을 수령했습니다.", item = mail.ItemName, quantity = mail.Quantity });
        }
        
        // 우편 삭제
        [HttpDelete("delete/{mailId}")]
        public async Task<IActionResult> DeleteMail(int mailId)
        {
            var mail = await _dbContext.Mails.FindAsync(mailId);
            if (mail == null)
                return NotFound(new { success = false, message = "우편을 찾을 수 없습니다." });

            _dbContext.Mails.Remove(mail);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "우편이 삭제되었습니다." });
        }
    }
}
