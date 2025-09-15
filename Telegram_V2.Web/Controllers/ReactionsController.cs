using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

[ApiController]
[Route("api/messages/{messageId}/reactions")]
public class ReactionsController : ControllerBase
{
    private readonly Context _context;

    public ReactionsController(Context context)
    {
        _context = context;
    }

    // ✅ Xabarga reaction qo'shish
    [HttpPost]
    public async Task<IActionResult> AddReaction(int messageId, [FromBody] ReactionDto dto)
    {
        // Reaction allaqachon mavjudmi tekshirish
        var existingReaction = await _context.MessageReactions
            .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == dto.UserId);

        if (existingReaction != null)
        {
            // Agar reaction o'zgartirilayotgan bo'lsa
            existingReaction.ReactionType = dto.ReactionType;
            existingReaction.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            // Yangi reaction
            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = dto.UserId,
                ReactionType = dto.ReactionType,
                CreatedAt = DateTime.UtcNow
            };
            _context.MessageReactions.Add(reaction);
        }

        await _context.SaveChangesAsync();
        return Ok("Reaction qo'shildi");
    }

    // ✅ Xabar reaksiyalarini olish
    [HttpGet]
    public async Task<IActionResult> GetMessageReactions(int messageId)
    {
        var reactions = await _context.MessageReactions
            .Where(r => r.MessageId == messageId)
            .Include(r => r.User)
            .Select(r => new
            {
                r.Id,
                r.ReactionType,
                User = new { r.User.Id, r.User.UserName }
            })
            .ToListAsync();

        return Ok(reactions);
    }

    // ✅ Reactionni olib tashlash
    [HttpDelete("{reactionId}")]
    public async Task<IActionResult> RemoveReaction(int messageId, int reactionId)
    {
        var reaction = await _context.MessageReactions
            .FirstOrDefaultAsync(r => r.Id == reactionId && r.MessageId == messageId);

        if (reaction == null) return NotFound("Reaction topilmadi");

        _context.MessageReactions.Remove(reaction);
        await _context.SaveChangesAsync();

        return Ok("Reaction olib tashlandi");
    }
}

