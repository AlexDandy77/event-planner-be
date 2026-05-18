using EventPlanner.Application.Users.Repositories;
using EventPlanner.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(EventPlannerDbContext dbContext) : IUserRepository
{
    private readonly EventPlannerDbContext _dbContext = dbContext;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmailForLookup(email);

        if (normalizedEmail is null)
        {
            return Task.FromResult<User?>(null);
        }

        return _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken
        );
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmailForLookup(email);

        if (normalizedEmail is null)
        {
            return Task.FromResult(false);
        }

        return _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeEmailForLookup(string email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
    }
}
