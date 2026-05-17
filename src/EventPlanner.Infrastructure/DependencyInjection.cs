using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Events.Repositories;
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

        services.AddDbContext<EventPlannerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
