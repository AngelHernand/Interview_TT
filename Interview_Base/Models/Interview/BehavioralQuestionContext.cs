using System.Collections.Generic;

namespace Interview_Base.Models.Interview;

public class BehavioralQuestionContext
{
    public string QuestionText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Competency { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public List<string> EvaluationCriteria { get; set; } = new();
    public List<string> RedFlags { get; set; } = new();
}
