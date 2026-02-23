namespace Interview_Base.DTOs.User;

public class UserCreateDto
{
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RolId { get; set; }
}
