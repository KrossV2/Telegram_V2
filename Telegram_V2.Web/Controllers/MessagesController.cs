using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;
using Telegram_V2.Core.Enums;
using Telegram_V2.Core.Dtos;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly Context _context;

    public MessagesController(Context context)
    {
        _context = context;
    }

    // ✅ Xabarlarni qidirish (XATO TO'GRILANDI)
    // ✅ Xabarlarni qidirish (ALTERNATIVE USUL)
    [HttpGet("search")]
    public async Task<IActionResult> SearchMessages([FromQuery] string text, [FromQuery] int? chatId = null)
    {
        if (string.IsNullOrEmpty(text)) return BadRequest("Qidiruv so'rovi bo'sh");

        // Asosiy queryni yaratish
        IQueryable<Message> query = _context.Messages;

        // Shartlarni qo'shish
        query = query.Where(m => m.Text.Contains(text));

        if (chatId.HasValue)
        {
            query = query.Where(m => m.ChatId == chatId.Value);
        }

        // Include larni oxiriga qo'shish
        var messages = await query
            .Include(m => m.Sender)
            .Include(m => m.Chat)
            .OrderByDescending(m => m.CreatedAt)
            .Take(50)
            .Select(m => new
            {
                m.Id,
                m.Text,
                m.CreatedAt,
                Sender = new { m.Sender.Id, m.Sender.UserName },
                ChatId = m.ChatId
            })
            .ToListAsync();

        return Ok(messages);
    }

    // ✅ Private xabar yuborish
    [HttpPost("private")]
    public async Task<IActionResult> SendPrivateMessage([FromBody] MessageCreateDto dto)
    {
        var message = new Message
        {
            ChatId = dto.ChatId,
            SenderId = dto.SenderId,
            Text = dto.Text,
            FileUrl = dto.FileUrl,
            ReplyToMessageId = dto.ReplyToMessageId,
            CreatedAt = DateTime.UtcNow,
            IsEdited = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Xabar statusini yaratish (yuborilgan)
        var status = new MessageStatus
        {
            MessageId = message.Id,
            UserId = dto.SenderId,
            Status = Status.Sent,
            UpdatedAt = DateTime.UtcNow
        };

        _context.MessageStatuses.Add(status);
        await _context.SaveChangesAsync();

        return Ok(new { MessageId = message.Id });
    }

    // ✅ Chatdagi barcha xabarlarni olish
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatMessages(int chatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new
            {
                m.Id,
                m.Text,
                m.FileUrl,
                m.IsEdited,
                m.CreatedAt,
                Sender = new { m.Sender.Id, m.Sender.UserName, m.Sender.ProfilePhotoUrl },
                ReplyTo = m.ReplyTo != null ? new { m.ReplyTo.Id, m.ReplyTo.Text } : null
            })
            .ToListAsync();

        return Ok(messages);
    }

    // ✅ Bitta xabarni olish
    [HttpGet("{chatId}/{messageId}")]
    public async Task<IActionResult> GetMessage(int chatId, int messageId)
    {
        var message = await _context.Messages
            .Where(m => m.Id == messageId && m.ChatId == chatId)
            .Include(m => m.Sender)
            .Select(m => new
            {
                m.Id,
                m.Text,
                m.FileUrl,
                m.IsEdited,
                m.CreatedAt,
                Sender = new { m.Sender.Id, m.Sender.UserName },
                Reactions = m.Reactions.Select(r => new { r.UserId, r.ReactionType })
            })
            .FirstOrDefaultAsync();

        if (message == null) return NotFound("Xabar topilmadi");
        return Ok(message);
    }

    // ✅ Xabarni tahrirlash
    [HttpPut("{messageId}")]
    public async Task<IActionResult> EditMessage(int messageId, [FromBody] MessageEditDto dto)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null) return NotFound("Xabar topilmadi");

        message.Text = dto.Text;
        message.IsEdited = true;
        await _context.SaveChangesAsync();

        return Ok("Xabar tahrirlandi");
    }

    // ✅ Xabarni o'chirish
    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null) return NotFound("Xabar topilmadi");

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        return Ok("Xabar o'chirildi");
    }

    // ✅ Xabarni o'qilgan deb belgilash
    [HttpPost("{messageId}/read")]
    public async Task<IActionResult> MarkAsRead(int messageId, [FromQuery] int userId)
    {
        var status = await _context.MessageStatuses
            .FirstOrDefaultAsync(ms => ms.MessageId == messageId && ms.UserId == userId);

        if (status == null)
        {
            status = new MessageStatus
            {
                MessageId = messageId,
                UserId = userId,
                Status = Status.Read,
                UpdatedAt = DateTime.UtcNow
            };
            _context.MessageStatuses.Add(status);
        }
        else
        {
            status.Status = Status.Read;
            status.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok("Xabar o'qilgan deb belgilandi");
    }

    // ✅ Chatdagi barcha xabar statuslari
    [HttpGet("{chatId}/statuses")]
    public async Task<IActionResult> GetMessageStatuses(int chatId)
    {
        var statuses = await _context.MessageStatuses
            .Where(ms => ms.Message.ChatId == chatId)
            .Include(ms => ms.User)
            .Include(ms => ms.Message)
            .Select(ms => new
            {
                MessageId = ms.MessageId,
                UserId = ms.UserId,
                UserName = ms.User.UserName,
                Status = ms.Status.ToString(),
                UpdatedAt = ms.UpdatedAt
            })
            .ToListAsync();

        return Ok(statuses);
    }

    // ✅ Xabarni pin qilish
    [HttpPost("{messageId}/pin")]
    public async Task<IActionResult> PinMessage(int messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Chat)
            .FirstOrDefaultAsync(m => m.Id == messageId);
        if (message == null) return NotFound("Xabar topilmadi");

        var existingPin = await _context.PinnedMessages
            .FirstOrDefaultAsync(p => p.ChatId == message.ChatId && p.MessageId == messageId);
        if (existingPin != null) return BadRequest("Xabar allaqachon pin qilingan");

        var pin = new PinnedMessage
        {
            ChatId = message.ChatId,
            MessageId = messageId
        };

        _context.PinnedMessages.Add(pin);
        await _context.SaveChangesAsync();

        return Ok("Xabar pin qilindi");
    }

    // ✅ Pinned xabarni olib tashlash
    [HttpDelete("{messageId}/unpin")]
    public async Task<IActionResult> UnpinMessage(int messageId)
    {
        var pin = await _context.PinnedMessages
            .FirstOrDefaultAsync(p => p.MessageId == messageId);
        if (pin == null) return NotFound("Pin topilmadi");

        _context.PinnedMessages.Remove(pin);
        await _context.SaveChangesAsync();

        return Ok("Pin olib tashlandi");
    }

    // ✅ Chatdagi pinned xabarlarni olish
    [HttpGet("{chatId}/pinned")]
    public async Task<IActionResult> GetPinnedMessages(int chatId)
    {
        var pinnedMessages = await _context.PinnedMessages
            .Where(p => p.ChatId == chatId)
            .Include(p => p.Message)
            .ThenInclude(m => m.Sender)
            .Select(p => new
            {
                p.Message.Id,
                p.Message.Text,
                p.Message.CreatedAt,
                Sender = new { p.Message.Sender.Id, p.Message.Sender.UserName }
            })
            .ToListAsync();

        return Ok(pinnedMessages);
    }
}