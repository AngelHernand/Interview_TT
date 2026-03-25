using System;
using System.Collections.Generic;

namespace Interview_Base.Models;

public partial class InterviewSession
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public string TipoEntrevista { get; set; } = null!; // "tecnica", "behavioral", "mixta"

    public string? Tecnologia { get; set; }

    public string Nivel { get; set; } = null!; // "Junior", "Mid", "Senior"

    public string Estado { get; set; } = null!; // "EnCurso", "Finalizada", "Cancelada"

    public string SystemPrompt { get; set; } = null!;

    public string? ContextoRecuperado { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? EvaluacionJson { get; set; }

    public int DuracionMinutos { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<InterviewMessage> Mensajes { get; set; } = new List<InterviewMessage>();
}
