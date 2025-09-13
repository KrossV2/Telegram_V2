namespace Telegram_V2.Core.Dtos;

public class FileAttachmentDto
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
}