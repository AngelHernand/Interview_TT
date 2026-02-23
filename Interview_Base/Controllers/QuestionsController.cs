using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Interview_Base.Data;
using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.Questions;

namespace Interview_Base.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class QuestionsController : ControllerBase
{
    //Obtiene las 15 preguntas del test
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<List<QuestionDto>>), StatusCodes.Status200OK)]
    public IActionResult GetQuestions()
    {
        var questions = QuestionBank.GetQuestions();
        return Ok(ApiResponseDto<List<QuestionDto>>.Ok(questions, "Preguntas obtenidas exitosamente."));
    }

    //Evalúa las respuestas del test y devuelve el resultado
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(ApiResponseDto<TestResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<TestResultDto>), StatusCodes.Status400BadRequest)]
    public IActionResult Evaluate([FromBody] List<AnswerDto> answers)
    {
        if (answers == null || answers.Count == 0)
        {
            return BadRequest(ApiResponseDto<TestResultDto>.Fail("Debe enviar al menos una respuesta."));
        }

        var result = QuestionBank.Evaluate(answers);
        return Ok(ApiResponseDto<TestResultDto>.Ok(result, "Test evaluado exitosamente."));
    }
}
