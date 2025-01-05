using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;
using ServerStudy.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace ServerStudy.Controllers;

public class ClanController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    private int? GetUserId()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
    
    public ClanController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Clan>>> GetClans()
    {
        return await _context.Clans
            .Include(c => c.Leader)
            .ToListAsync();
    }
    
    //클랜 생성
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateClan([FromBody] Clan clan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
    
        clan.LeaderId = userId ?? 0;
        
        _context.Clans.Add(clan);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetClans), new { id = clan.ClanId }, clan);
    }

    //클랜 삭제
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteClan(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        
        //클랜 조회
        var clan = await _context.Clans.FindAsync(id);
        
        if (clan == null)
        {
            return NotFound();
        }
        //클랜 리더만 삭제 가능
        if (clan.LeaderId != userId)
        {
            return Forbid();
        }
        
        _context.Clans.Remove(clan);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }

    //리더 변경
    [HttpPut("{id}/change-leader")]
    [Authorize]
    public async Task<IActionResult> ChangeLeader(int id, [FromBody] int newLeaderId)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        
        //클랜 조회
        var clan = await _context.Clans.Include(c => c.Users).FirstOrDefaultAsync(c => c.LeaderId == id);
        if (clan == null)
        {
            return NotFound();
        }
        
        //리더인지 확인
        if (clan.LeaderId != userId)
        {
            return Forbid();
        }
        
        //클랜원인지 확인
        var newLeader = clan.Users.FirstOrDefault(m => m.UserId == newLeaderId);
        if (newLeader == null)
        {
            return BadRequest("클랜에 소속된 클랜원이 아님");
        }
        
        clan.LeaderId = newLeaderId;
        _context.Clans.Update(clan);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "리더 변경 완료" });
    }

}