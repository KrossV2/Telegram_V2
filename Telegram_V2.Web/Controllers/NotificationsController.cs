using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly Context _context;

    public NotificationsController(Context context)
    {
        _context = context;
    }

    // ✅ Foydalanuvchi bildirishnomalari
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(100)
            .Select(n => new
            {
                n.Id,
                n.Type,
                n.Message,
                n.IsRead,
                n.CreatedAt
            })
            .ToListAsync();

        return Ok(notifications);
    }

    // ✅ Bildirishnomani o'qilgan deb belgilash
    [HttpPost("mark-read/{id}")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null) return NotFound("Bildirishnoma topilmadi");

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok("Bildirishnoma o'qilgan deb belgilandi");
    }
}