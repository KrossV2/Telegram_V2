using Telegram_V2.Core.Enums;

namespace Telegram_V2.Core.Models;

public class MessageStatus
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public Status Status { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Message Message { get; set; }
    public Users User { get; set; }
}