namespace Telegram_V2.Core.Dtos;

public class GroupCreateDto
{
    public string GroupName { get; set; }
    public string? Description { get; set; }
    public int CreatedById { get; set; }
}