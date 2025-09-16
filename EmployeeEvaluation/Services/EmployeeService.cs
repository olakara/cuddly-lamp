using EmployeeEvaluation.Models;
using Serilog;

namespace EmployeeEvaluation.Services;

public class EmployeeService
{
    private readonly List<Employee> _employees = new();

    public EmployeeService()
    {
        Log.Information("Initializing EmployeeService with sample employees");
        
        // Initialize with some sample employees
        _employees.AddRange(new[]
        {
            new Employee { EmployeeNumber = "EMP001", Name = "John Doe", Department = "Engineering" },
            new Employee { EmployeeNumber = "EMP002", Name = "Jane Smith", Department = "Marketing" },
            new Employee { EmployeeNumber = "EMP003", Name = "Bob Johnson", Department = "Engineering" },
            new Employee { EmployeeNumber = "EMP004", Name = "Alice Brown", Department = "HR" }
        });
        
        Log.Information("EmployeeService initialized with {EmployeeCount} employees", _employees.Count);
    }

    public Employee? GetEmployee(string employeeNumber)
    {
        Log.Debug("Searching for employee with number: {EmployeeNumber}", employeeNumber);
        
        var employee = _employees.FirstOrDefault(e => e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase));
        
        if (employee != null)
        {
            Log.Information("Employee found: {EmployeeName} ({EmployeeNumber})", employee.Name, employee.EmployeeNumber);
        }
        else
        {
            Log.Warning("Employee not found with number: {EmployeeNumber}", employeeNumber);
        }
        
        return employee;
    }

    public List<Employee> GetAllEmployees()
    {
        Log.Information("Retrieving all employees. Retrieved {EmployeeCount} employees.", _employees.Count);
        return _employees.ToList();
    }

    public void AddEmployee(Employee employee)
    {
        Log.Information("Attempting to add new employee: {EmployeeName} ({EmployeeNumber})", employee.Name, employee.EmployeeNumber);
        
        if (_employees.Any(e => e.EmployeeNumber.Equals(employee.EmployeeNumber, StringComparison.OrdinalIgnoreCase)))
        {
            Log.Error("Attempted to add duplicate employee with number: {EmployeeNumber}", employee.EmployeeNumber);
            throw new InvalidOperationException($"Employee with number {employee.EmployeeNumber} already exists.");
        }
        
        _employees.Add(employee);
        Log.Information("Successfully added employee: {EmployeeName} ({EmployeeNumber}) in {Department}", 
            employee.Name, employee.EmployeeNumber, employee.Department);
    }
}