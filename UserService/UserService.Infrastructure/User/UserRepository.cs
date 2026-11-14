using Microsoft.EntityFrameworkCore;
using UserService.Application;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Id == id,
                cancellationToken);
    }
    
    public async Task<User?> GetByLoginAsync(
        string login,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                u => u.Login == login,
                cancellationToken);
    }
}