using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Telegram_V2.Application.Settings;
using Telegram_V2.Core.Models;

namespace Telegram_V2.Application.Services;

public interface IAuthService
{
    string GetToken(Users username);
}

public class AuthService(IOptions<JwtSettings> settings) : IAuthService
{
    private readonly IOptions<JwtSettings> _settings = settings;
    private readonly JwtSecurityTokenHandler _handler = new();

    public string GetToken(Users user)
    {
        var claims = new List<Claim>
    {
        new Claim("user_id", user.Id.ToString()),

        // null bo‘lsa string.Empty qo‘yib yuboramiz
        new Claim("email", user.Email ?? string.Empty),
        new Claim("username", user.UserName ?? string.Empty),
        new Claim("firstname", user.FirstName ?? string.Empty),
        new Claim("lastname", user.LastName ?? string.Empty),
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TelegramV2",
            audience: "TelegramV2",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials
        );

        return _handler.WriteToken(token);
    }

}