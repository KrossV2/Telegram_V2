namespace Telegram_V2.Core.Dtos;

public class SignInResponseDto
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public int ExpiresIn { get; set; }
}
