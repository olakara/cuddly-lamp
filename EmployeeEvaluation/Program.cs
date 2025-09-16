using EmployeeEvaluation.Services;
using EmployeeEvaluation.UI;
using Serilog;


// Initialize services
var employeeService = new EmployeeService();
var dataPersistenceService = new DataPersistenceService();
var evaluationService = new EvaluationService(dataPersistenceService, employeeService);

// Configure Serilog with rolling file logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        path: "../logs/application.log",
        rollingInterval: RollingInterval.Infinite,
        rollOnFileSizeLimit: true,
        fileSizeLimitBytes: 2 * 1024 * 1024, // 2 MB
        retainedFileCountLimit: 10,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();


try
{
    Log.Information("Employee Evaluation System starting up");

    // Initialize services
    var employeeService = new EmployeeService();
    var evaluationService = new EvaluationService();

    Log.Information("Services initialized successfully");

    // Create and run the console interface
    var consoleInterface = new ConsoleInterface(employeeService, evaluationService);
    consoleInterface.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Employee Evaluation System shutting down");
    Log.CloseAndFlush();
}
