namespace Telegram_V2.Core.Dtos;

public class MessageUpdateDto
{
    public string? Text { get; set; }
    public string? FileUrl { get; set; }
    public bool? IsEdited { get; set; }
}