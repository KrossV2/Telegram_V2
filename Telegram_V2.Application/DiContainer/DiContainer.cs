using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegram_V2.Application.Services;
using Microsoft.AspNetCore.Identity;
using Telegram_V2.Core.Models;

namespace Telegram_V2.Application.DiContainer;

public static class DiContainer
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService , AuthService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();

        return services;
    }
}
