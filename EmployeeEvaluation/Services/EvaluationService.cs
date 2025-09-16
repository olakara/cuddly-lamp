using EmployeeEvaluation.Models;
using Serilog;

namespace EmployeeEvaluation.Services;

public class EvaluationService
{
    private readonly List<Evaluation> _evaluations = new();
    private readonly DataPersistenceService _dataPersistenceService;
    private readonly EmployeeService _employeeService;

    public EvaluationService(DataPersistenceService dataPersistenceService, EmployeeService employeeService)
    {
        _dataPersistenceService = dataPersistenceService;
        _employeeService = employeeService;
    }

    public Evaluation CreateEvaluation(string employeeNumber, int quarter, int year)
    {
        Log.Information("Creating evaluation for employee {EmployeeNumber} for Q{Quarter} {Year}", employeeNumber, quarter, year);
        
        var existingEvaluation = GetEvaluation(employeeNumber, quarter, year);
        if (existingEvaluation != null)
        {
            Log.Error("Attempted to create duplicate evaluation for employee {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
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
        Log.Information("Successfully created evaluation for employee {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
        return evaluation;
    }

    public Evaluation? GetEvaluation(string employeeNumber, int quarter, int year)
    {
        Log.Debug("Searching for evaluation: {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
        
        var evaluation = _evaluations.FirstOrDefault(e => 
            e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase) && 
            e.Quarter == quarter && 
            e.Year == year);
            
        if (evaluation != null)
        {
            Log.Debug("Found evaluation for {EmployeeNumber} Q{Quarter} {Year}, Status: {Status}", 
                employeeNumber, quarter, year, evaluation.IsCompleted ? "Completed" : "Pending");
        }
        else
        {
            Log.Debug("No evaluation found for {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
        }
        
        return evaluation;
    }

    public List<Evaluation> GetEmployeeEvaluations(string employeeNumber)
    {
        Log.Information("Retrieving all evaluations for employee {EmployeeNumber}", employeeNumber);
        
        var evaluations = _evaluations
            .Where(e => e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.Year)
            .ThenByDescending(e => e.Quarter)
            .ToList();
            
        Log.Information("Found {EvaluationCount} evaluations for employee {EmployeeNumber}", evaluations.Count, employeeNumber);
        return evaluations;
    }

    public List<Evaluation> GetPendingEvaluations()
    {
        Log.Information("Retrieving all pending evaluations");
        
        var pendingEvaluations = _evaluations.Where(e => !e.IsCompleted).ToList();
        Log.Information("Found {PendingCount} pending evaluations", pendingEvaluations.Count);
        
        return pendingEvaluations;
    }

    public void SubmitSelfEvaluation(string employeeNumber, int quarter, int year, Dictionary<EvaluationCategory, int> scores)
    {
        Log.Information("Submitting self-evaluation for employee {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
        
        var evaluation = GetEvaluation(employeeNumber, quarter, year);
        if (evaluation == null)
        {
            Log.Information("No existing evaluation found, creating new one for {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
            evaluation = CreateEvaluation(employeeNumber, quarter, year);
        }

        ValidateScores(scores);
        evaluation.SelfScores = new Dictionary<EvaluationCategory, int>(scores);
        

        // Save to JSON file after self-evaluation completion
        var employee = _employeeService.GetEmployee(employeeNumber);
        if (employee != null)
        {
            // Use synchronous version for simplicity in console app
            _dataPersistenceService.SaveEvaluationAsync(evaluation, employee).Wait();
        }

        Log.Information("Successfully submitted self-evaluation for employee {EmployeeNumber} Q{Quarter} {Year} with scores: {@Scores}", 
            employeeNumber, quarter, year, scores);

    }

    public void SubmitManagerEvaluation(string employeeNumber, int quarter, int year, 
        Dictionary<EvaluationCategory, int> scores, string remarks)
    {
        Log.Information("Submitting manager evaluation for employee {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
        
        var evaluation = GetEvaluation(employeeNumber, quarter, year);
        if (evaluation == null)
        {
            Log.Error("No evaluation found for manager evaluation: {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
            throw new InvalidOperationException($"No evaluation found for employee {employeeNumber} in Q{quarter} {year}. Employee must submit self-evaluation first.");
        }

        if (!evaluation.HasSelfEvaluationCompleted())
        {
            Log.Error("Self-evaluation not completed for {EmployeeNumber} Q{Quarter} {Year}", employeeNumber, quarter, year);
            throw new InvalidOperationException("Employee must complete self-evaluation before manager can evaluate.");
        }

        ValidateScores(scores);
        evaluation.ManagerScores = new Dictionary<EvaluationCategory, int>(scores);
        evaluation.ManagerRemarks = remarks ?? string.Empty;
        evaluation.IsCompleted = true;
        evaluation.CompletedDate = DateTime.Now;
        

        // Save to JSON file after manager evaluation completion
        var employee = _employeeService.GetEmployee(employeeNumber);
        if (employee != null)
        {
            // Use synchronous version for simplicity in console app
            _dataPersistenceService.SaveEvaluationAsync(evaluation, employee).Wait();
        }

        Log.Information("Successfully completed manager evaluation for employee {EmployeeNumber} Q{Quarter} {Year} with scores: {@Scores} and remarks: {Remarks}", 
            employeeNumber, quarter, year, scores, remarks);

    }

    private static void ValidateScores(Dictionary<EvaluationCategory, int> scores)
    {
        Log.Debug("Validating evaluation scores");
        
        var requiredCategories = Enum.GetValues<EvaluationCategory>();
        
        foreach (var category in requiredCategories)
        {
            if (!scores.ContainsKey(category))
            {
                Log.Error("Missing score for category: {Category}", category.GetDisplayName());
                throw new ArgumentException($"Score for {category.GetDisplayName()} is required.");
            }

            var score = scores[category];
            if (score < 1 || score > 5)
            {
                Log.Error("Invalid score {Score} for category {Category}. Must be between 1 and 5", score, category.GetDisplayName());
                throw new ArgumentException($"Score for {category.GetDisplayName()} must be between 1 and 5.");
            }
        }
        
        Log.Debug("All scores validated successfully");
    }
}