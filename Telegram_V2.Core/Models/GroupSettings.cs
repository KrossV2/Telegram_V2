namespace Telegram_V2.Core.Models;

public class GroupSettings
{
    public int ChatId { get; set; }
    public string GroupName { get; set; }
    public string? GroupPhotoUrl { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Chat Chat { get; set; }
}