using EventPlanner.Infrastructure.Persistence;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventPlanner.IntegrationTests.Support;

public sealed class EventPlannerApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"event-planner-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",
                    ["Jwt:Issuer"] = "EventPlanner.Api",
                    ["Jwt:Audience"] = "EventPlanner.Client",
                    ["Jwt:Secret"] = "development-only-jwt-secret-change-before-production-1234567890",
                    ["Jwt:AccessTokenMinutes"] = "60",
                    ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
                }
            );
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<EventPlannerDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<EventPlannerDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<EventPlannerDbContext>>();
            services.AddDbContext<EventPlannerDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EventPlannerDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });
    }
}
