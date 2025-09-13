namespace Telegram_V2.Core.Dtos;

public class ChannelDto
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string ChannelName { get; set; }
    public string? Description { get; set; }
}

