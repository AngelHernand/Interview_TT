using System;

namespace Interview_Base.Models;

// Modelo generado por scaffold — NO MODIFICAR
public partial class LoginAttempt
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public bool Exitoso { get; set; }

    public string? DireccionIp { get; set; }

    public string? UserAgent { get; set; }

    public string? MensajeError { get; set; }

    public DateTime Fecha { get; set; }
}
