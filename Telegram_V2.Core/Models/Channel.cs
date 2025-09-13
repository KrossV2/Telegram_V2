namespace Telegram_V2.Core.Models;

public class Channel
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string ChannelName { get; set; }
    public string? ChannelPhotoUrl { get; set; }
    public string? Description { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Chat Chat { get; set; }
    public Users CreatedBy { get; set; }
}