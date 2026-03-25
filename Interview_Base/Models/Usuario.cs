using System;
using System.Collections.Generic;

namespace Interview_Base.Models;

/// Modelo generado por scaffold — NO MODIFICAR
public partial class Usuario
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RolId { get; set; }

    public bool Activo { get; set; }

    public bool Bloqueado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }

    public int IntentosLogin { get; set; }

    public DateTime? FechaBloqueo { get; set; }

    public virtual Rol Rol { get; set; } = null!;

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<InterviewSession> InterviewSessions { get; set; } = new List<InterviewSession>();
}
