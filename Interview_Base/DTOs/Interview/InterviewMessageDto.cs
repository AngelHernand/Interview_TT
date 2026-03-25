using System;

namespace Interview_Base.DTOs.Interview;

public class InterviewMessageDto
{
    public string Rol { get; set; } = string.Empty;  // "entrevistador" | "candidato"
    public string Contenido { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
