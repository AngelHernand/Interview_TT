namespace Interview_Base.Helpers;

/// Utilidades para hasheo y verificación de contraseñas con BCrypt.
public static class PasswordHelper
{
    private const int WorkFactor = 11;

    /// Genera el hash BCrypt de una contraseña en texto plano.
    public static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    // Verifica si una contraseña coincide con el hash almacenado.
    public static bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
