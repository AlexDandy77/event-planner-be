using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;
using EventPlanner.Infrastructure.Persistence;
using EventPlanner.Infrastructure.Persistence.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.UnitTests.Infrastructure;

public sealed class UserRepositoryTests
{
    private static readonly DateTimeOffset CreatedAt = DateTimeOffset.Parse(
        "2026-05-21T09:00:00Z"
    );

    [Fact]
    public async Task GetByEmailAsync_ShouldFindUserIgnoringWhitespaceAndCase()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var user = await repository.GetByEmailAsync(" ADMIN@EXAMPLE.COM ", CancellationToken.None);

        Assert.NotNull(user);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal(UserRole.Admin, user.Role);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var exists = await repository.ExistsByEmailAsync("missing@example.com", CancellationToken.None);

        Assert.False(exists);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldThrow_WhenCancellationIsRequestedBeforeValidationReturn()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var getUser = () =>
            repository.GetByEmailAsync(" ", cancellationTokenSource.Token);

        await Assert.ThrowsAsync<OperationCanceledException>(getUser);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUserWithNormalizedEmail()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var user = User.Create(
            "Test",
            "Organizer",
            "New.Organizer@Example.COM",
            UserRole.Organizer,
            "+37360009999",
            "development-password-hash",
            CreatedAt
        );

        await repository.AddAsync(user, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var persistedUser = await repository.GetByEmailAsync(
            "new.organizer@example.com",
            CancellationToken.None
        );

        Assert.NotNull(persistedUser);
        Assert.Equal(user.Id, persistedUser.Id);
        Assert.Equal("new.organizer@example.com", persistedUser.Email);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldThrow_WhenEmailIsDuplicated()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var duplicateUser = User.Create(
            "Duplicate",
            "Admin",
            "ADMIN@example.com",
            UserRole.Admin,
            null,
            "development-password-hash",
            CreatedAt
        );

        await repository.AddAsync(duplicateUser, CancellationToken.None);

        await Assert.ThrowsAsync<DbUpdateException>(() =>
            repository.SaveChangesAsync(CancellationToken.None)
        );
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
