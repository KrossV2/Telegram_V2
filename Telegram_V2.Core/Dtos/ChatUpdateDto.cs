namespace Telegram_V2.Core.Dtos;

public class ChatUpdateDto
{
    public bool? IsGroup { get; set; }
    public string? ChatName { get; set; } // Grup nomi uchun
    public string? ChatPhotoUrl { get; set; } // Grup rasmi uchun
}