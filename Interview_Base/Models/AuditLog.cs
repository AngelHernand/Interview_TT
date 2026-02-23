using System;

namespace Interview_Base.Models;


// Modelo generado por scaffold — NO MODIFICAR
public partial class AuditLog
{
    public long Id { get; set; }

    public Guid? UsuarioId { get; set; }

    public string Accion { get; set; } = null!;

    public string? Entidad { get; set; }

    public string? EntidadId { get; set; }

    public string? ValoresAnteriores { get; set; }

    public string? ValoresNuevos { get; set; }

    public string? DireccionIp { get; set; }

    public string? UserAgent { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
