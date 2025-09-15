using Telegram_V2.Core.Enums;

namespace Telegram_V2.Core.Dtos;

public class CallEndDto
{
    public TimeSpan Duration { get; set; }
    public CallStatus CallStatus { get; set; }
}