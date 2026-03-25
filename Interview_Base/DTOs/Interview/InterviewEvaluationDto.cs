using System.Collections.Generic;

namespace Interview_Base.DTOs.Interview;

public class InterviewEvaluationDto
{
    public double PuntuacionGeneral { get; set; }
    public string NivelEvaluado { get; set; } = string.Empty;
    public List<CompetenciaEvaluadaDto> Competencias { get; set; } = new();
    public string ResumenGeneral { get; set; } = string.Empty;
    public List<string> Fortalezas { get; set; } = new();
    public List<string> AreasDeMejora { get; set; } = new();
    public string Recomendacion { get; set; } = string.Empty; // "Apto", "Con reservas", "No apto"
}

public class CompetenciaEvaluadaDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Puntuacion { get; set; } // 1-5
    public string Justificacion { get; set; } = string.Empty;
}
