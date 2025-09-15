using Telegram_V2.Core.Enums;

namespace Telegram_V2.Core.Dtos;
public class CallStartDto
{
    public int CallerId { get; set; }
    public int ReceiverId { get; set; }
    public CallType CallType { get; set; }
}