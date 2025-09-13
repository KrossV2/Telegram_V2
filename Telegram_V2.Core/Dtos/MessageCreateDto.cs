namespace Telegram_V2.Core.Dtos;

public class MessageCreateDto
{
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public string? Text { get; set; }
    public string? FileUrl { get; set; }
    public int? ReplyToMessageId { get; set; }
}