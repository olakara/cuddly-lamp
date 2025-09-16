using System.Text.Json;
using EmployeeEvaluation.Models;

namespace EmployeeEvaluation.Services;

public class DataPersistenceService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public DataPersistenceService(string filePath = "evaluations.json")
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task SaveEvaluationAsync(Evaluation evaluation, Employee employee)
    {
        var evaluationData = CreateEvaluationData(evaluation, employee);
        var allEvaluations = await LoadAllEvaluationsAsync();
        
        // Remove existing evaluation for the same employee, quarter, and year
        allEvaluations.RemoveAll(e => 
            e.EmployeeNumber == evaluation.EmployeeNumber && 
            e.Quarter == evaluation.Quarter && 
            e.Year == evaluation.Year);
        
        // Add the new evaluation
        allEvaluations.Add(evaluationData);
        
        // Save to file
        var json = JsonSerializer.Serialize(allEvaluations, _jsonOptions);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private async Task<List<EvaluationData>> LoadAllEvaluationsAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<EvaluationData>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            var evaluations = JsonSerializer.Deserialize<List<EvaluationData>>(json, _jsonOptions);
            return evaluations ?? new List<EvaluationData>();
        }
        catch
        {
            // If file is corrupted or empty, start fresh
            return new List<EvaluationData>();
        }
    }

    private static EvaluationData CreateEvaluationData(Evaluation evaluation, Employee employee)
    {
        return new EvaluationData
        {
            EmployeeNumber = evaluation.EmployeeNumber,
            EmployeeName = employee.Name,
            EmployeeDepartment = employee.Department,
            Quarter = evaluation.Quarter,
            Year = evaluation.Year,
            SelfScores = evaluation.SelfScores.ToDictionary(
                kvp => kvp.Key.ToString(), 
                kvp => kvp.Value),
            ManagerScores = evaluation.ManagerScores.ToDictionary(
                kvp => kvp.Key.ToString(), 
                kvp => kvp.Value),
            ManagerRemarks = evaluation.ManagerRemarks,
            IsCompleted = evaluation.IsCompleted,
            CreatedDate = evaluation.CreatedDate,
            CompletedDate = evaluation.CompletedDate
        };
    }
}

public class EvaluationData
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeDepartment { get; set; } = string.Empty;
    public int Quarter { get; set; }
    public int Year { get; set; }
    public Dictionary<string, int> SelfScores { get; set; } = new();
    public Dictionary<string, int> ManagerScores { get; set; } = new();
    public string ManagerRemarks { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}