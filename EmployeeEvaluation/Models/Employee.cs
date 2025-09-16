namespace EmployeeEvaluation.Models;

public class Employee
{
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{EmployeeNumber} - {Name} ({Department})";
    }
}