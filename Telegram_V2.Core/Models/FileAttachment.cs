namespace Telegram_V2.Core.Models;


public class FileAttachment
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }

    // Navigation
    public Message Message { get; set; }
}