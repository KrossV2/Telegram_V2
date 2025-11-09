namespace Telegram_V2.Application.Exceptions;

public class AuthException : Exception
{
    public AuthException(string message) : base(message)
    {
    }
}
