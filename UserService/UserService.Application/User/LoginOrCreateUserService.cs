using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Application.Users;

public class LoginOrCreateUserService
{
    private readonly IUserRepository _userRepository;

    public LoginOrCreateUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> LoginOrCreateAsync(
        string login,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login cannot be empty.");

        login = login.Trim();

        var user = await _userRepository.GetByLoginAsync(
            login,
            cancellationToken);

        if (user != null)
            return user;

        user = new User
        {
            Id = Guid.NewGuid(),
            Login = login
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return user;
    }
}