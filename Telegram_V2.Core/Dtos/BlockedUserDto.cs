

namespace Telegram_V2.Core.Dtos;

public class BlockedUserDto
{
    public int Id { get; set; }
    public int BlockedUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}