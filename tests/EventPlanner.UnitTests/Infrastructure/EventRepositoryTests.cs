using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Queries;
using EventPlanner.Infrastructure.Persistence;
using EventPlanner.Infrastructure.Persistence.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.UnitTests.Infrastructure;

public sealed class EventRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ShouldThrow_WhenCancellationIsRequested()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new EventRepository(database.Context);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var getEvents = () =>
            repository.GetAllAsync(
                new EventQuery(
                    From: null,
                    To: null,
                    Category: null,
                    Search: null,
                    SortBy: EventSortFields.StartDateTime,
                    SortDirection: SortDirections.Asc
                ),
                cancellationTokenSource.Token
            );

        await Assert.ThrowsAsync<OperationCanceledException>(getEvents);
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private TestDatabase(SqliteConnection connection, EventPlannerDbContext context)
        {
            Connection = connection;
            Context = context;
        }

        public EventPlannerDbContext Context { get; }

        private SqliteConnection Connection { get; }

        public static async Task<TestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<EventPlannerDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new EventPlannerDbContext(options);
            await context.Database.EnsureCreatedAsync();

            return new TestDatabase(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }
}
