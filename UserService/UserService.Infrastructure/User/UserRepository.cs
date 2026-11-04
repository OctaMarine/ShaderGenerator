using UserService.Application;
using UserService.Domain.Entities;


namespace UserService.Infrastructure.User;

public class UserRepository : IUserRepository
{
    public Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken)
    {
        
    }

    public Task<Domain.Entities.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        
    }
}