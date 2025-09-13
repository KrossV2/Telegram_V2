namespace Telegram_V2.Core.Dtos;

public class CallHistoryDto
{
    public int Id { get; set; }
    public int CallerId { get; set; }
    public int RecieverId { get; set; }
    public string CallType { get; set; }
    public string CallStatus { get; set; }
    public TimeSpan Duration { get; set; }
}
