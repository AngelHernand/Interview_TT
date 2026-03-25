using Interview_Base.Models.Interview;

namespace Interview_Base.Services.Interfaces;

public interface IBehavioralQuestionProvider
{
    Task<List<BehavioralQuestionContext>> GetRandomQuestionsAsync(int count, string? difficulty = null);
    Task<List<string>> GetAvailableCategoriesAsync();
}
