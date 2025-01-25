using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerStudy.DataBase;
using ServerStudy.Models;

namespace ServerStudy.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerMatchController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private static readonly object Lock = new(); // 동기화를 위한 Lock

    public PlayerMatchController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost("enqueue")]
    public async Task<IActionResult> Enqueue([FromBody] Player player)
    {
        player.EnqueuedAt = DateTime.UtcNow;
        player.IsMatched = false;

        _dbContext.Players.Add(player);
        await _dbContext.SaveChangesAsync();

        return Ok(new { success = true, message = "플레이어가 대기열에 추가되었습니다." });
    }

    // 2.대기열에서 플레이어 제거
    [HttpPost("dequeue/{playerId}")]
    public async Task<IActionResult> Dequeue(int playerId)
    {
        var player = await _dbContext.Players.FindAsync(playerId);
        if (player == null)
        {
            return NotFound(new { success = false, message = "플레이어를 찾을 수 없습니다." });
        }
        
        lock (Lock)
        {
            _dbContext.Players.Remove(player);
        }
        
        await _dbContext.SaveChangesAsync();

        return Ok(new { success = true, message = "플레이어가 대기열에서 제거되었습니다." });
    }

    // 3.매칭 상태 확인
    [HttpGet("status/{playerId}")]
    public async Task<IActionResult> CheckStatus(int playerId)
    {
        var player = await _dbContext.Players.FindAsync(playerId);
        if (player == null)
        {
            return NotFound(new { success = false, message = "플레이어를 찾을 수 없습니다." });
        }

        if (player.IsMatched)
        {
            var match = await _dbContext.Matches
                .Include(m => m.Players)
                .Where(m => m.Players.Any(p => p.Id == playerId))
                .FirstOrDefaultAsync();


            return Ok(new { success = true, matched = true, match });
        }

        return Ok(new { success = true, matched = false });
    }

    // 4. 매칭 처리
    [HttpPost("process")]
    public async Task<IActionResult> ProcessMatching()
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var players = await _dbContext.Players
            .Where(p => !p.IsMatched)
            .OrderBy(p => p.EnqueuedAt)
            .Take(2)
            .ToListAsync();

        if (players.Count < 2)
        {
            return BadRequest(new { success = false, message = "매칭을 위해 플레이어가 부족합니다." });
        }

        var match = new Match
        {
            Players = players,
            MatchedAt = DateTime.UtcNow
        };

        foreach (var player in players)
        {
            player.IsMatched = true;
        }

        _dbContext.Matches.Add(match);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new { success = true, message = "매칭이 완료되었습니다." });

    }
    
    // 대기열이 긴 플레이어 제거
    [HttpPost("cleanup")]
    public async Task<IActionResult> CleanupQueue()
    {
        var timeoutThreshold = TimeSpan.FromMinutes(5);
        var cutoffTime = DateTime.UtcNow - timeoutThreshold;

        var expiredPlayers = await _dbContext.Players
            .Where(p => !p.IsMatched && p.EnqueuedAt < cutoffTime)
            .ToListAsync();

        if (expiredPlayers.Any())
        {
            _dbContext.Players.RemoveRange(expiredPlayers);
            await _dbContext.SaveChangesAsync();
        }

        return Ok(new { success = true, message = "타임아웃된 플레이어가 정리되었습니다.", removedCount = expiredPlayers.Count });
    }
}