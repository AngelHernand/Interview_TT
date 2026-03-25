namespace Interview_Base.DTOs.Interview;

public class StartInterviewRequestDto
{
    public string TipoEntrevista { get; set; } = null!; // "tecnica", "behavioral", "mixta"
    public string? Tecnologia { get; set; }             // ".NET", "C#", "SQL", etc.
    public string Nivel { get; set; } = null!;          // "Junior", "Mid", "Senior"
    public int DuracionMinutos { get; set; } = 30;
}
