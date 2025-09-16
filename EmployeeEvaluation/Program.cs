using EmployeeEvaluation.Services;
using EmployeeEvaluation.UI;

// Initialize services
var employeeService = new EmployeeService();
var dataPersistenceService = new DataPersistenceService();
var evaluationService = new EvaluationService(dataPersistenceService, employeeService);

// Create and run the console interface
var consoleInterface = new ConsoleInterface(employeeService, evaluationService);
consoleInterface.Run();
