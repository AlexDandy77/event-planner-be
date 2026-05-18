using EventPlanner.Application.Auth.Abstractions;
using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Application.Users.Repositories;
using EventPlanner.Infrastructure.Auth;
using EventPlanner.Infrastructure.Persistence;
using EventPlanner.Infrastructure.Persistence.Repositories;
using EventPlanner.Infrastructure.Time;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured."
            );
        }

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtSection["Issuer"] ?? string.Empty;
            options.Audience = jwtSection["Audience"] ?? string.Empty;
            options.Secret = jwtSection["Secret"] ?? string.Empty;
            options.AccessTokenMinutes = int.TryParse(
                jwtSection["AccessTokenMinutes"],
                out var accessTokenMinutes
            )
                ? accessTokenMinutes
                : 60;
        });
        services.AddDbContext<EventPlannerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IAuthTokenService, JwtTokenService>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
