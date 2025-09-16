using EmployeeEvaluation.Services;
using EmployeeEvaluation.UI;

// Initialize services
var employeeService = new EmployeeService();
var evaluationService = new EvaluationService();

// Create and run the console interface
var consoleInterface = new ConsoleInterface(employeeService, evaluationService);
consoleInterface.Run();
