using UserService.Domain.Entities;

namespace UserService.Application;

public interface IUserRepository
{
    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}