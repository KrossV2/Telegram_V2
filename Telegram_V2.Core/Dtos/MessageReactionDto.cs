namespace Telegram_V2.Core.Dtos;

public class MessageReactionDto
{
    public int Id { get; set; }
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string ReactionType { get; set; }
}
