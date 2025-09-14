using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Telegram_V2.Application.Services;

namespace Telegram_V2.Application.DiContainer;

public static class DiContainer
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService , AuthService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
