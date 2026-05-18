using EventPlanner.Application.Auth.Services;
using EventPlanner.Application.Events.Services;

using Microsoft.Extensions.DependencyInjection;

namespace EventPlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}
