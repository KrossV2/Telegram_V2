using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public ICollection<Contact> Contacts { get; set; }
    [JsonIgnore]
    public ICollection<ChatParticipant> ChatParticipants { get; set; }
    [JsonIgnore]
    public ICollection<Message> Messages { get; set; }
    [JsonIgnore]
    public ICollection<Notification> Notifications { get; set; }
    [JsonIgnore]
    public ICollection<MessageReaction> Reactions { get; set; }
    [JsonIgnore]
    public ICollection<MessageStatus> MessageStatuses { get; set; }
    [JsonIgnore]
    public ICollection<BlockedUser> BlockedUsers { get; set; }
    [JsonIgnore]
    public ICollection<CallHistory> CallHistories { get; set; }
}