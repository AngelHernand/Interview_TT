namespace Interview_Base.DTOs.Questions;

//Representa una pregunta del test con sus opciones
public class QuestionDto
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public List<OptionDto> Opciones { get; set; } = new();
}

//Opción de respuesta para una pregunta
public class OptionDto
{
    public string Clave { get; set; } = string.Empty;   // A, B, C, D
    public string Texto { get; set; } = string.Empty;
}

//Respuesta enviada por el usuario para una pregunta
public class AnswerDto
{
    public int PreguntaId { get; set; }
    public string ClaveSeleccionada { get; set; } = string.Empty;
}

//Resultado del test completo
public class TestResultDto
{
    public int TotalPreguntas { get; set; }
    public int Correctas { get; set; }
    public int Incorrectas { get; set; }
    public double PorcentajeAcierto { get; set; }
    public List<QuestionResultDto> Detalle { get; set; } = new();
}

//Resultado individual de cada pregunta
public class QuestionResultDto
{
    public int PreguntaId { get; set; }
    public string TextoPregunta { get; set; } = string.Empty;
    public string ClaveSeleccionada { get; set; } = string.Empty;
    public string ClaveCorrecta { get; set; } = string.Empty;
    public bool EsCorrecta { get; set; }
}
