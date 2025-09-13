namespace Telegram_V2.Core.Dtos;

public class CallHistoryCreateDto
{
    public int CallerId { get; set; }
    public int RecieverId { get; set; }
    public string CallType { get; set; }
}
