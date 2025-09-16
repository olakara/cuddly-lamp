using EmployeeEvaluation.Models;
using EmployeeEvaluation.Services;

namespace EmployeeEvaluation.UI;

public class ConsoleInterface
{
    private readonly EmployeeService _employeeService;
    private readonly EvaluationService _evaluationService;

    public ConsoleInterface(EmployeeService employeeService, EvaluationService evaluationService)
    {
        _employeeService = employeeService;
        _evaluationService = evaluationService;
    }

    public void Run()
    {
        Console.WriteLine("=== Employee Evaluation System ===");
        Console.WriteLine();

        while (true)
        {
            ShowMainMenu();
            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        PerformSelfEvaluation();
                        break;
                    case "2":
                        PerformManagerEvaluation();
                        break;
                    case "3":
                        ViewEvaluations();
                        break;
                    case "4":
                        ViewEmployees();
                        break;
                    case "5":
                        Console.WriteLine("Thank you for using the Employee Evaluation System!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private void ShowMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("1. Employee Self-Evaluation");
        Console.WriteLine("2. Manager Evaluation");
        Console.WriteLine("3. View Evaluations");
        Console.WriteLine("4. View Employees");
        Console.WriteLine("5. Exit");
        Console.WriteLine();
        Console.Write("Please select an option (1-5): ");
    }

    private void PerformSelfEvaluation()
    {
        Console.WriteLine("\n=== Employee Self-Evaluation ===");
        
        var employee = SelectEmployee();
        if (employee == null) return;

        var (quarter, year) = GetQuarterAndYear();
        
        Console.WriteLine($"\nSelf-evaluation for {employee.Name} - {GetQuarterDisplay(quarter, year)}");
        Console.WriteLine("Please rate yourself on a scale of 1-5 for each category:");
        Console.WriteLine("(1 = Poor, 2 = Below Average, 3 = Average, 4 = Good, 5 = Excellent)");
        Console.WriteLine();

        var scores = new Dictionary<EvaluationCategory, int>();
        
        foreach (var category in Enum.GetValues<EvaluationCategory>())
        {
            while (true)
            {
                Console.Write($"{category.GetDisplayName()}: ");
                if (int.TryParse(Console.ReadLine(), out int score) && score >= 1 && score <= 5)
                {
                    scores[category] = score;
                    break;
                }
                Console.WriteLine("Please enter a valid score between 1 and 5.");
            }
        }

        _evaluationService.SubmitSelfEvaluation(employee.EmployeeNumber, quarter, year, scores);
        Console.WriteLine("\nSelf-evaluation submitted successfully!");
    }

    private void PerformManagerEvaluation()
    {
        Console.WriteLine("\n=== Manager Evaluation ===");
        
        var pendingEvaluations = _evaluationService.GetPendingEvaluations()
            .Where(e => e.HasSelfEvaluationCompleted()).ToList();

        if (!pendingEvaluations.Any())
        {
            Console.WriteLine("No pending evaluations available for manager review.");
            return;
        }

        Console.WriteLine("Evaluations pending manager review:");
        for (int i = 0; i < pendingEvaluations.Count; i++)
        {
            var eval = pendingEvaluations[i];
            var employee = _employeeService.GetEmployee(eval.EmployeeNumber);
            Console.WriteLine($"{i + 1}. {employee?.Name} ({eval.EmployeeNumber}) - {eval.GetQuarterDisplay()}");
        }

        Console.Write("\nSelect evaluation to review (1-{0}): ", pendingEvaluations.Count);
        if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > pendingEvaluations.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var selectedEval = pendingEvaluations[choice - 1];
        var selectedEmployee = _employeeService.GetEmployee(selectedEval.EmployeeNumber);

        Console.WriteLine($"\nManager evaluation for {selectedEmployee?.Name} - {selectedEval.GetQuarterDisplay()}");
        Console.WriteLine("\nEmployee's Self-Evaluation:");
        foreach (var kvp in selectedEval.SelfScores)
        {
            Console.WriteLine($"{kvp.Key.GetDisplayName()}: {kvp.Value}");
        }

        Console.WriteLine("\nPlease provide your evaluation (1-5 for each category):");
        var managerScores = new Dictionary<EvaluationCategory, int>();
        
        foreach (var category in Enum.GetValues<EvaluationCategory>())
        {
            while (true)
            {
                Console.Write($"{category.GetDisplayName()} (Employee: {selectedEval.SelfScores[category]}): ");
                if (int.TryParse(Console.ReadLine(), out int score) && score >= 1 && score <= 5)
                {
                    managerScores[category] = score;
                    break;
                }
                Console.WriteLine("Please enter a valid score between 1 and 5.");
            }
        }

        Console.Write("\nManager remarks: ");
        var remarks = Console.ReadLine() ?? string.Empty;

        _evaluationService.SubmitManagerEvaluation(selectedEmployee!.EmployeeNumber, 
            selectedEval.Quarter, selectedEval.Year, managerScores, remarks);
        
        Console.WriteLine("\nManager evaluation completed successfully!");
    }

    private void ViewEvaluations()
    {
        Console.WriteLine("\n=== View Evaluations ===");
        
        var employee = SelectEmployee();
        if (employee == null) return;

        var evaluations = _evaluationService.GetEmployeeEvaluations(employee.EmployeeNumber);
        
        if (!evaluations.Any())
        {
            Console.WriteLine($"No evaluations found for {employee.Name}.");
            return;
        }

        Console.WriteLine($"\nEvaluations for {employee.Name}:");
        Console.WriteLine();

        foreach (var eval in evaluations)
        {
            Console.WriteLine($"=== {eval.GetQuarterDisplay()} ===");
            Console.WriteLine($"Status: {(eval.IsCompleted ? "Completed" : "Pending")}");
            Console.WriteLine($"Created: {eval.CreatedDate:yyyy-MM-dd}");
            if (eval.CompletedDate.HasValue)
                Console.WriteLine($"Completed: {eval.CompletedDate:yyyy-MM-dd}");

            if (eval.HasSelfEvaluationCompleted())
            {
                Console.WriteLine("\nSelf-Evaluation:");
                foreach (var kvp in eval.SelfScores)
                {
                    Console.WriteLine($"  {kvp.Key.GetDisplayName()}: {kvp.Value}");
                }
            }

            if (eval.HasManagerEvaluationCompleted())
            {
                Console.WriteLine("\nManager Evaluation:");
                foreach (var kvp in eval.ManagerScores)
                {
                    Console.WriteLine($"  {kvp.Key.GetDisplayName()}: {kvp.Value}");
                }
                if (!string.IsNullOrWhiteSpace(eval.ManagerRemarks))
                {
                    Console.WriteLine($"\nManager Remarks: {eval.ManagerRemarks}");
                }
            }
            Console.WriteLine();
        }
    }

    private void ViewEmployees()
    {
        Console.WriteLine("\n=== All Employees ===");
        var employees = _employeeService.GetAllEmployees();
        
        foreach (var employee in employees)
        {
            Console.WriteLine(employee.ToString());
        }
    }

    private Employee? SelectEmployee()
    {
        var employees = _employeeService.GetAllEmployees();
        
        Console.WriteLine("\nSelect an employee:");
        for (int i = 0; i < employees.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {employees[i]}");
        }

        Console.Write($"\nEnter choice (1-{employees.Count}): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= employees.Count)
        {
            return employees[choice - 1];
        }

        Console.WriteLine("Invalid selection.");
        return null;
    }

    private static (int quarter, int year) GetQuarterAndYear()
    {
        int quarter, year;
        
        while (true)
        {
            Console.Write("Enter quarter (1-4): ");
            if (int.TryParse(Console.ReadLine(), out quarter) && quarter >= 1 && quarter <= 4)
                break;
            Console.WriteLine("Please enter a valid quarter (1-4).");
        }

        while (true)
        {
            Console.Write("Enter year (e.g., 2024): ");
            if (int.TryParse(Console.ReadLine(), out year) && year >= 2020 && year <= 2030)
                break;
            Console.WriteLine("Please enter a valid year between 2020 and 2030.");
        }

        return (quarter, year);
    }

    private static string GetQuarterDisplay(int quarter, int year)
    {
        return $"Q{quarter} {year}";
    }
}