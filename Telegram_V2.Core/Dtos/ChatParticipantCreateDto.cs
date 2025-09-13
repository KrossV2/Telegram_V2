namespace Telegram_V2.Core.Dtos;


public class ChatParticipantCreateDto
{
    public int ChatId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
}