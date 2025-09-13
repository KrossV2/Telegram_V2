namespace Telegram_V2.Core.Dtos;

public class GroupSettingsDto
{
    public int GroupId { get; set; }
    public int ChatId { get; set; }
    public string GroupName { get; set; }
    public string? GroupPhotoUrl { get; set; }
    public string? Description { get; set; }
}