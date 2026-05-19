using EventPlanner.Application.Users.Repositories;
using EventPlanner.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(EventPlannerDbContext dbContext) : IUserRepository
{
    private readonly EventPlannerDbContext _dbContext = dbContext;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedEmail = NormalizeEmailForLookup(email);

        if (normalizedEmail is null)
        {
            return null;
        }

        return await _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken
        );
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var normalizedEmail = NormalizeEmailForLookup(email);

        if (normalizedEmail is null)
        {
            return false;
        }

        return await _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeEmailForLookup(string email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
    }
}
