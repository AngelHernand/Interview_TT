using System;

namespace Interview_Base.Models;

// Modelo de la vista vw_UsuariosActivos — generado por scaffold — NO MODIFICAR
public partial class VwUsuariosActivo
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? UltimoAcceso { get; set; }
}
