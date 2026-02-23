using System;

namespace Interview_Base.Models;

// Modelo generado por scaffold — NO MODIFICAR
public partial class RefreshToken
{
    public int Id { get; set; }

    public Guid UsuarioId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime FechaExpiracion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool Revocado { get; set; }

    public DateTime? FechaRevocacion { get; set; }

    public string? ReemplazadoPor { get; set; }

    public string? DireccionIp { get; set; }

    public string? UserAgent { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
