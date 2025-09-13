namespace Telegram_V2.Core.Models;

public class MessageReaction
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string? ReactionType { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Message Message { get; set; }
    public Users User { get; set; }
}