namespace Telegram_V2.Core.Dtos;

public class MessageReactionCreateDto
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string ReactionType { get; set; }
}
