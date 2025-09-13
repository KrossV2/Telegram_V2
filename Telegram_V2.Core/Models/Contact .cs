namespace Telegram_V2.Core.Models;

public class Contact
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ContactUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Users User { get; set; }
    public Users ContactUser { get; set; }
}
