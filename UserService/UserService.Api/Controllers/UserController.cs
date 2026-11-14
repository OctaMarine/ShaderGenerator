using Microsoft.AspNetCore.Mvc;
using UserService.App.DTO;
using UserService.Application.Users;

namespace UserService.App.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly LoginOrCreateUserService _loginService;

    public UserController(LoginOrCreateUserService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _loginService.LoginOrCreateAsync(
            request.Login,
            cancellationToken);

        var response = new LoginResponse(user.Id, user.Login);

        return Ok(response);
    }
}