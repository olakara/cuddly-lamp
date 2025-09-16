using EmployeeEvaluation.Models;

namespace EmployeeEvaluation.Services;

public class EvaluationService
{
    private readonly List<Evaluation> _evaluations = new();

    public Evaluation CreateEvaluation(string employeeNumber, int quarter, int year)
    {
        var existingEvaluation = GetEvaluation(employeeNumber, quarter, year);
        if (existingEvaluation != null)
        {
            throw new InvalidOperationException($"Evaluation for employee {employeeNumber} in Q{quarter} {year} already exists.");
        }

        var evaluation = new Evaluation
        {
            EmployeeNumber = employeeNumber,
            Quarter = quarter,
            Year = year,
            CreatedDate = DateTime.Now
        };

        _evaluations.Add(evaluation);
        return evaluation;
    }

    public Evaluation? GetEvaluation(string employeeNumber, int quarter, int year)
    {
        return _evaluations.FirstOrDefault(e => 
            e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase) && 
            e.Quarter == quarter && 
            e.Year == year);
    }

    public List<Evaluation> GetEmployeeEvaluations(string employeeNumber)
    {
        return _evaluations
            .Where(e => e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.Year)
            .ThenByDescending(e => e.Quarter)
            .ToList();
    }

    public List<Evaluation> GetPendingEvaluations()
    {
        return _evaluations.Where(e => !e.IsCompleted).ToList();
    }

    public void SubmitSelfEvaluation(string employeeNumber, int quarter, int year, Dictionary<EvaluationCategory, int> scores)
    {
        var evaluation = GetEvaluation(employeeNumber, quarter, year);
        if (evaluation == null)
        {
            evaluation = CreateEvaluation(employeeNumber, quarter, year);
        }

        ValidateScores(scores);
        evaluation.SelfScores = new Dictionary<EvaluationCategory, int>(scores);
    }

    public void SubmitManagerEvaluation(string employeeNumber, int quarter, int year, 
        Dictionary<EvaluationCategory, int> scores, string remarks)
    {
        var evaluation = GetEvaluation(employeeNumber, quarter, year);
        if (evaluation == null)
        {
            throw new InvalidOperationException($"No evaluation found for employee {employeeNumber} in Q{quarter} {year}. Employee must submit self-evaluation first.");
        }

        if (!evaluation.HasSelfEvaluationCompleted())
        {
            throw new InvalidOperationException("Employee must complete self-evaluation before manager can evaluate.");
        }

        ValidateScores(scores);
        evaluation.ManagerScores = new Dictionary<EvaluationCategory, int>(scores);
        evaluation.ManagerRemarks = remarks ?? string.Empty;
        evaluation.IsCompleted = true;
        evaluation.CompletedDate = DateTime.Now;
    }

    private static void ValidateScores(Dictionary<EvaluationCategory, int> scores)
    {
        var requiredCategories = Enum.GetValues<EvaluationCategory>();
        
        foreach (var category in requiredCategories)
        {
            if (!scores.ContainsKey(category))
            {
                throw new ArgumentException($"Score for {category.GetDisplayName()} is required.");
            }

            var score = scores[category];
            if (score < 1 || score > 5)
            {
                throw new ArgumentException($"Score for {category.GetDisplayName()} must be between 1 and 5.");
            }
        }
    }
}