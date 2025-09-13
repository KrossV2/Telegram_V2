namespace Telegram_V2.Core.Models;

public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public string? Text { get; set; }
    public string? FileUrl { get; set; }
    public int? ReplyToMessageId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Chat Chat { get; set; }
    public Users Sender { get; set; }
    public Message ReplyTo { get; set; }
    public ICollection<FileAttachment> Attachments { get; set; }
    public ICollection<MessageReaction> Reactions { get; set; }
    public ICollection<MessageStatus> Statuses { get; set; }
}