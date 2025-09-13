namespace Telegram_V2.Core.Models;

public class Users
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<Contact> Contacts { get; set; }
    public ICollection<ChatParticipant> ChatParticipants { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<MessageReaction> Reactions { get; set; }
    public ICollection<MessageStatus> MessageStatuses { get; set; }
    public ICollection<BlockedUser> BlockedUsers { get; set; }
    public ICollection<CallHistory> CallHistories { get; set; }
}