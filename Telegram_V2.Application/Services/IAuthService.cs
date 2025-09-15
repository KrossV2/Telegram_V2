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
        new Claim("email", user.Email),
        new Claim("username", user.UserName),
        new Claim("firstname", user.FirstName),
        new Claim("lastname", user.LastName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "TelegramV2",
            "TelegramV2",
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
            signingCredentials: credentials
        );

        return _handler.WriteToken(token);
    }
}