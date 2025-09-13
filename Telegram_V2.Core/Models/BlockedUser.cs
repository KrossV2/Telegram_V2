
namespace Telegram_V2.Core.Models;

public class BlockedUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BlockedUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Users User { get; set; }
    public Users Blocked { get; set; }
}