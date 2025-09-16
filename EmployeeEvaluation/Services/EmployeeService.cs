using EmployeeEvaluation.Models;

namespace EmployeeEvaluation.Services;

public class EmployeeService
{
    private readonly List<Employee> _employees = new();

    public EmployeeService()
    {
        // Initialize with some sample employees
        _employees.AddRange(new[]
        {
            new Employee { EmployeeNumber = "EMP001", Name = "John Doe", Department = "Engineering" },
            new Employee { EmployeeNumber = "EMP002", Name = "Jane Smith", Department = "Marketing" },
            new Employee { EmployeeNumber = "EMP003", Name = "Bob Johnson", Department = "Engineering" },
            new Employee { EmployeeNumber = "EMP004", Name = "Alice Brown", Department = "HR" }
        });
    }

    public Employee? GetEmployee(string employeeNumber)
    {
        return _employees.FirstOrDefault(e => e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase));
    }

    public List<Employee> GetAllEmployees()
    {
        return _employees.ToList();
    }

    public void AddEmployee(Employee employee)
    {
        if (_employees.Any(e => e.EmployeeNumber.Equals(employee.EmployeeNumber, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Employee with number {employee.EmployeeNumber} already exists.");
        }
        _employees.Add(employee);
    }
}