namespace Telegram_V2.Core.Models;

public class PinnedMessage
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int MessageId { get; set; }

    // Navigation
    public Chat Chat { get; set; }
    public Message Message { get; set; }
}