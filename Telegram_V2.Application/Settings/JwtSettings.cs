namespace Telegram_V2.Application.Settings;

public class JwtSettings
{
    public string Key { get; set; }

    public string[] Issuers { get; set; }

    public string[] Audiences { get; set; }
}