using System.ComponentModel.DataAnnotations;

namespace UserService.App.DTO;

public record LoginRequest(
    [Required] string Login);