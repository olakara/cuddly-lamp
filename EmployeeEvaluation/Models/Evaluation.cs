namespace EmployeeEvaluation.Models;

public class Evaluation
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public int Quarter { get; set; }
    public int Year { get; set; }
    public Dictionary<EvaluationCategory, int> SelfScores { get; set; } = new();
    public Dictionary<EvaluationCategory, int> ManagerScores { get; set; } = new();
    public string ManagerRemarks { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public string GetQuarterDisplay()
    {
        return $"Q{Quarter} {Year}";
    }

    public bool HasSelfEvaluationCompleted()
    {
        return SelfScores.Count == Enum.GetValues<EvaluationCategory>().Length;
    }

    public bool HasManagerEvaluationCompleted()
    {
        return ManagerScores.Count == Enum.GetValues<EvaluationCategory>().Length;
    }
}