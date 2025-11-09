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