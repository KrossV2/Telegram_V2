namespace Telegram_V2.Core.Dtos;

public class ChatDto
{
    public int Id { get; set; }
    public bool IsGroup { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
}