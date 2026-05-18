using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
