namespace Telegram_V2.Core.Models;

public class ChatParticipant
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }

    // Navigation
    public Chat Chat { get; set; }
    public Users User { get; set; }
}