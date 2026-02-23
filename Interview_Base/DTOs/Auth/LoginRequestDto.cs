using System.ComponentModel.DataAnnotations;

namespace Interview_Base.DTOs.Auth;

public class LoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
