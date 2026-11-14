using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IUserRepository
{
    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
    Task<User?> GetByLoginAsync(
        string login,
        CancellationToken cancellationToken);
}