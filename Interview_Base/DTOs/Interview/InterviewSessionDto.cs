using System;
using System.Collections.Generic;

namespace Interview_Base.DTOs.Interview;

public class InterviewSessionDto
{
    public Guid Id { get; set; }
    public string TipoEntrevista { get; set; } = string.Empty;
    public string? Tecnologia { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int DuracionMinutos { get; set; }
    public int TotalMensajes { get; set; }
    public List<InterviewMessageDto> Mensajes { get; set; } = new();
}

public class InterviewSessionListDto
{
    public Guid Id { get; set; }
    public string TipoEntrevista { get; set; } = string.Empty;
    public string? Tecnologia { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int TotalMensajes { get; set; }
    public double? PuntuacionGeneral { get; set; }
}
