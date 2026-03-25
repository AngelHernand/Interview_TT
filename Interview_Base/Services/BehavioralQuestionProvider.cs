using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Interview_Base.Data;
using Interview_Base.Models.Interview;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Services;

public class BehavioralQuestionProvider : IBehavioralQuestionProvider
{
    private readonly ScrapingReadDbContext _context;
    private readonly ILogger<BehavioralQuestionProvider> _logger;

    public BehavioralQuestionProvider(ScrapingReadDbContext context, ILogger<BehavioralQuestionProvider> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<BehavioralQuestionContext>> GetRandomQuestionsAsync(int count, string? difficulty = null)
    {
        var query = _context.BehavioralQuestions
            .Where(q => q.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(difficulty))
            query = query.Where(q => q.Difficulty == difficulty);

        // Muestreo estratificado: 1 por categoría, luego tomar N al azar
        var onePerCategory = await query
            .GroupBy(q => q.Category)
            .Select(g => g.OrderBy(_ => EF.Functions.Random()).First())
            .ToListAsync();

        var selectedIds = onePerCategory
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .Select(q => q.Id)
            .ToList();

        // Cargar con includes
        var questions = await _context.BehavioralQuestions
            .Where(q => selectedIds.Contains(q.Id))
            .Include(q => q.EvaluationCriteria.OrderBy(e => e.OrderIndex))
            .Include(q => q.RedFlags)
            .AsNoTracking()
            .ToListAsync();

        _logger.LogInformation("Se obtuvieron {Count} preguntas behavioral (dificultad: {Difficulty})",
            questions.Count, difficulty ?? "todas");

        return questions.Select(q => new BehavioralQuestionContext
        {
            QuestionText = q.QuestionText,
            Category = q.Category,
            Competency = q.Competency,
            Difficulty = q.Difficulty,
            EvaluationCriteria = q.EvaluationCriteria.Select(e => e.CriteriaText).ToList(),
            RedFlags = q.RedFlags.Select(r => r.FlagText).ToList()
        }).ToList();
    }

    public async Task<List<string>> GetAvailableCategoriesAsync()
    {
        return await _context.BehavioralQuestions
            .Where(q => q.IsActive)
            .Select(q => q.Category)
            .Distinct()
            .OrderBy(c => c)
            .AsNoTracking()
            .ToListAsync();
    }
}
