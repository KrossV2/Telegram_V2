using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

[ApiController]
[Route("api/chats")]
public class ChatsController : ControllerBase
{
    private readonly Context _context;

    public ChatsController(Context context)
    {
        _context = context;
    }

    // ✅ Userning gruplari (XATO TO'GRILANDI)
    [HttpGet("group/{userId}")]
    public async Task<IActionResult> GetUserGroups(int userId)
    {
        var groups = await _context.ChatParticipants
            .Where(cp => cp.UserId == userId && cp.Chat.IsGroup)
            .Include(cp => cp.Chat)
            .ThenInclude(c => c.Participants)
            .Select(cp => new
            {
                GroupId = cp.ChatId,
                GroupName = cp.Chat.ChatName, // GroupSettings o'rniga ChatName
                GroupPhoto = cp.Chat.ChatPhotoUrl,
                MembersCount = cp.Chat.Participants.Count,
                LastMessage = cp.Chat.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new { m.Text, m.CreatedAt })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(groups);
    }

    // ✅ 1:1 chat yaratish yoki mavjudini qaytarish
    [HttpPost("private")]
    public async Task<IActionResult> CreatePrivateChat([FromBody] PrivateChatCreateDto dto)
    {
        // Avval mavjud chatni tekshirish
        var existingChat = await _context.ChatParticipants
            .Where(cp => cp.UserId == dto.User1Id)
            .Where(cp => _context.ChatParticipants
                .Where(cp2 => cp2.UserId == dto.User2Id)
                .Select(cp2 => cp2.ChatId)
                .Contains(cp.ChatId))
            .Select(cp => cp.Chat)
            .FirstOrDefaultAsync();

        if (existingChat != null)
        {
            return Ok(new
            {
                ChatId = existingChat.Id,
                IsNew = false
            });
        }

        // Yangi chat yaratish
        var chat = new Chat
        {
            IsGroup = false,
            CreatedById = dto.User1Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        // Participantlarni qo'shish
        var participants = new[]
        {
            new ChatParticipant { ChatId = chat.Id, UserId = dto.User1Id, Role = "member", JoinedAt = DateTime.UtcNow },
            new ChatParticipant { ChatId = chat.Id, UserId = dto.User2Id, Role = "member", JoinedAt = DateTime.UtcNow }
        };

        _context.ChatParticipants.AddRange(participants);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            ChatId = chat.Id,
            IsNew = true
        });
    }

    // ✅ Foydalanuvchining barcha private chatlari
    [HttpGet("private/{userId}")]
    public async Task<IActionResult> GetUserPrivateChats(int userId)
    {
        var chats = await _context.ChatParticipants
            .Where(cp => cp.UserId == userId && !cp.Chat.IsGroup)
            .Include(cp => cp.Chat)
            .ThenInclude(c => c.Participants)
            .ThenInclude(p => p.User)
            .Select(cp => new
            {
                ChatId = cp.ChatId,
                Participants = cp.Chat.Participants
                    .Where(p => p.UserId != userId)
                    .Select(p => new
                    {
                        p.UserId,
                        p.User.UserName,
                        p.User.ProfilePhotoUrl,
                        p.User.IsOnline
                    }),
                LastMessage = cp.Chat.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new { m.Text, m.CreatedAt })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(chats);
    }

    // ✅ Yangi grup yaratish
    [HttpPost("group")]
    public async Task<IActionResult> CreateGroup([FromBody] GroupCreateDto dto)
    {
        var chat = new Chat
        {
            IsGroup = true,
            CreatedById = dto.CreatedById,
            CreatedAt = DateTime.UtcNow,
            ChatName = dto.GroupName
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        // Group settings yaratish
        var groupSettings = new GroupSettings
        {
            ChatId = chat.Id,
            GroupName = dto.GroupName,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.GroupSettings.Add(groupSettings);

        // Creator ni participant qilish
        var participant = new ChatParticipant
        {
            ChatId = chat.Id,
            UserId = dto.CreatedById,
            Role = "admin",
            JoinedAt = DateTime.UtcNow
        };

        _context.ChatParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return Ok(new { GroupId = chat.Id });
    }

    // ✅ Userni grupga qo'shish
    [HttpPost("group/{groupId}/add-user")]
    public async Task<IActionResult> AddUserToGroup(int groupId, [FromBody] AddUserToGroupDto dto)
    {
        // Grup mavjudligini tekshirish
        var group = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == groupId && c.IsGroup);
        if (group == null) return NotFound("Grup topilmadi");

        // User allaqachon gruptami tekshirish
        var existingParticipant = await _context.ChatParticipants
            .FirstOrDefaultAsync(cp => cp.ChatId == groupId && cp.UserId == dto.UserId);
        if (existingParticipant != null) return BadRequest("User allaqachon grupta");

        var participant = new ChatParticipant
        {
            ChatId = groupId,
            UserId = dto.UserId,
            Role = "member",
            JoinedAt = DateTime.UtcNow
        };

        _context.ChatParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return Ok("User guruhga qo'shildi");
    }

    // ✅ Userni gruptan chiqarish
    [HttpDelete("group/{groupId}/remove-user")]
    public async Task<IActionResult> RemoveUserFromGroup(int groupId, [FromQuery] int userId)
    {
        var participant = await _context.ChatParticipants
            .FirstOrDefaultAsync(cp => cp.ChatId == groupId && cp.UserId == userId);

        if (participant == null) return NotFound("Participant topilmadi");

        _context.ChatParticipants.Remove(participant);
        await _context.SaveChangesAsync();

        return Ok("User guruhdan chiqarildi");
    }

    // ✅ Chatni o'chirish
    [HttpDelete("{chatId}")]
    public async Task<IActionResult> DeleteChat(int chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);
        if (chat == null) return NotFound("Chat topilmadi");

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();

        return Ok("Chat o'chirildi");
    }
}