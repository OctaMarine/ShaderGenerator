using Microsoft.AspNetCore.Mvc;

namespace UserService.App.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    [HttpGet("login")]
    public ActionResult Login()
    {
        return Ok();
    }
}