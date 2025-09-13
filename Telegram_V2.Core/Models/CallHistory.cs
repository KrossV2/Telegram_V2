using Telegram_V2.Core.Enums;

namespace Telegram_V2.Core.Models;

public class CallHistory
{
    public int Id { get; set; }
    public int CallerId { get; set; }
    public int RecieverId { get; set; }
    public CallType CallType { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public CallStatus CallStatus { get; set; }

    // Navigation
    public Users Caller { get; set; }
    public Users Receiver { get; set; }
}