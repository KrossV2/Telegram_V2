namespace Telegram_V2.Core.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsOnline { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime LastSeen { get; set; }
}