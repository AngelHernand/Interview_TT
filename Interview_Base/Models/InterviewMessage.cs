using System;

namespace Interview_Base.Models;

public partial class InterviewMessage
{
    public long Id { get; set; }

    public Guid SessionId { get; set; }

    public string Rol { get; set; } = null!; // "system", "entrevistador", "candidato"

    public string Contenido { get; set; } = null!;

    public int Orden { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual InterviewSession Session { get; set; } = null!;
}
