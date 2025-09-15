using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Dtos;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly Context _context;

    public UsersController(Context context)
    {
        _context = context;
    }

    // ✅ Barcha foydalanuvchilar ro'yxati
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.PhoneNumber,
                u.Email,
                u.ProfilePhotoUrl,
                u.IsOnline,
                u.LastSeen
            })
            .ToListAsync();

        return Ok(users);
    }

    // ✅ Bitta foydalanuvchi ma'lumotlari
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.PhoneNumber,
                u.Email,
                u.ProfilePhotoUrl,
                u.Bio,
                u.IsOnline,
                u.LastSeen,
                u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null) return NotFound("Foydalanuvchi topilmadi");
        return Ok(user);
    }

    // ✅ Foydalanuvchi ma'lumotlarini tahrirlash
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Foydalanuvchi topilmadi");

        // Faqat null emas propertylarni yangilash
        if (!string.IsNullOrEmpty(dto.UserName)) user.UserName = dto.UserName;
        if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.ProfilePhotoUrl)) user.ProfilePhotoUrl = dto.ProfilePhotoUrl;
        if (!string.IsNullOrEmpty(dto.Bio)) user.Bio = dto.Bio;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok("Ma'lumotlar yangilandi");
    }

    // ✅ Foydalanuvchini o'chirish
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Foydalanuvchi topilmadi");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok("Foydalanuvchi o'chirildi");
    }

    // ✅ Foydalanuvchini bloklash
    [HttpPost("{id}/block/{blockedUserId}")]
    public async Task<IActionResult> BlockUser(int id, int blockedUserId)
    {
        // Bloklovchi va bloklanuvchi mavjudligini tekshirish
        var blocker = await _context.Users.FindAsync(id);
        var blocked = await _context.Users.FindAsync(blockedUserId);
        if (blocker == null || blocked == null) return NotFound("Foydalanuvchi topilmadi");

        // Avval bloklanganmi tekshirish
        var existingBlock = await _context.BlockedUsers
            .FirstOrDefaultAsync(b => b.UserId == id && b.BlockedUserId == blockedUserId);

        if (existingBlock != null) return BadRequest("Ushbu foydalanuvchi allaqachon bloklangan");

        var block = new BlockedUser
        {
            UserId = id,
            BlockedUserId = blockedUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.BlockedUsers.Add(block);
        await _context.SaveChangesAsync();

        return Ok("Foydalanuvchi bloklandi");
    }

    // ✅ Blokdan chiqarish
    [HttpDelete("{id}/unblock/{blockedUserId}")]
    public async Task<IActionResult> UnblockUser(int id, int blockedUserId)
    {
        var block = await _context.BlockedUsers
            .FirstOrDefaultAsync(b => b.UserId == id && b.BlockedUserId == blockedUserId);

        if (block == null) return NotFound("Blok topilmadi");

        _context.BlockedUsers.Remove(block);
        await _context.SaveChangesAsync();

        return Ok("Blok olib tashlandi");
    }

    // ✅ Bloklangan foydalanuvchilar ro'yxati
    [HttpGet("{id}/blocked")]
    public async Task<IActionResult> GetBlockedUsers(int id)
    {
        var blockedUsers = await _context.BlockedUsers
            .Where(b => b.UserId == id)
            .Include(b => b.Blocked)
            .Select(b => new
            {
                b.BlockedUserId,
                b.Blocked.UserName,
                b.Blocked.PhoneNumber,
                b.CreatedAt
            })
            .ToListAsync();

        return Ok(blockedUsers);
    }

    // ✅ Userlarni qidirish
    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string name)
    {
        if (string.IsNullOrEmpty(name)) return BadRequest("Qidiruv so'rovi bo'sh");

        var users = await _context.Users
            .Where(u => u.UserName.Contains(name) || u.PhoneNumber.Contains(name))
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.PhoneNumber,
                u.ProfilePhotoUrl,
                u.IsOnline
            })
            .Take(50)
            .ToListAsync();

        return Ok(users);
    }

    // ✅ Profile photo yuklash (soddalashtirilgan versiya)
    [HttpPost("{id}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(int id, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Fayl yo'q");

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("Foydalanuvchi topilmadi");

        // Soddalashtirilgan: faqat URL ni saqlaymiz
        var fileName = $"user_{id}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
        user.ProfilePhotoUrl = $"/uploads/{fileName}";
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Haqiqiy faylni saqlash kodini qo'shishingiz mumkin
        return Ok(new { PhotoUrl = user.ProfilePhotoUrl });
    }
}

