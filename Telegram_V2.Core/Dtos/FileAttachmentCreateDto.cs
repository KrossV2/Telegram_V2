namespace Telegram_V2.Core.Dtos;

public class FileAttachmentCreateDto
{
    public int MessageId { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
}
