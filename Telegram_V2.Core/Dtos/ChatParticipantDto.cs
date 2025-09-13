namespace Telegram_V2.Core.Dtos;

public class ChatParticipantDto
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
}
