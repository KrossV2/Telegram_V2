namespace Telegram_V2.Core.Dtos;

public class NotificationCreateDto
{
    public int UserId { get; set; }
    public string Type { get; set; }
    public string Message { get; set; }
}