namespace Telegram_V2.Core.Dtos;


public class UserCreateDto
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsBot { get; set; }
}