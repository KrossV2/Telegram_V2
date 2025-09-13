namespace Telegram_V2.Core.Dtos;

public class MessageStatusCreateDto
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
}