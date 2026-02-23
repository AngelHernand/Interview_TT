namespace Interview_Base.DTOs.Auth;

public class RegisterRequestDto
{
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}
