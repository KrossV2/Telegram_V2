namespace Telegram_V2.Core.Models;


public class Chat
{
    public int Id { get; set; }
    public bool IsGroup { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ChatName { get; set; }
    public string? ChatPhotoUrl { get; set; }

    // Navigation
    public Users CreatedBy { get; set; }
    public ICollection<ChatParticipant> Participants { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<PinnedMessage> PinnedMessages { get; set; }

    // Group uchun propertylar
    public GroupSettings GroupSettings { get; set; }
}