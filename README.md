# Employee Evaluation System

A simple C# console application for managing quarterly employee evaluations.

## Features

- **Employee Management**: Pre-loaded with sample employees (Employee Number, Name, Department)
- **Self-Evaluation**: Employees can evaluate themselves across 5 categories
- **Manager Review**: Managers can review, modify scores, and add remarks
- **Evaluation Categories**: 
  - Attendance
  - Work Quality
  - Innovation
  - Communication
  - Skill Development
- **Score Range**: 1-5 scale (1 = Poor, 2 = Below Average, 3 = Average, 4 = Good, 5 = Excellent)
- **Evaluation History**: View completed and pending evaluations

## Requirements

- .NET 8.0 or later

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd cuddly-lamp
   ```

2. **Build the application**
   ```bash
   cd EmployeeEvaluation
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

## Usage

### Main Menu Options

1. **Employee Self-Evaluation**
   - Select an employee
   - Choose quarter and year
   - Rate yourself on each category (1-5)
   - Submit evaluation

2. **Manager Evaluation**
   - View pending evaluations that require manager review
   - See employee's self-evaluation scores
   - Provide manager scores for each category
   - Add remarks
   - Complete the evaluation

3. **View Evaluations**
   - Select an employee
   - See all evaluations (completed and pending)
   - View both self and manager scores
   - Read manager remarks

4. **View Employees**
   - See list of all employees in the system

## Workflow

1. **Employee submits self-evaluation** for a specific quarter
2. **Manager reviews** the self-evaluation and provides their assessment
3. **Manager adds remarks** and closes the evaluation
4. **Evaluation is marked as completed** and can be viewed in history

## Sample Employees

The system comes pre-loaded with these employees:
- EMP001 - John Doe (Engineering)
- EMP002 - Jane Smith (Marketing)
- EMP003 - Bob Johnson (Engineering)
- EMP004 - Alice Brown (HR)

## Architecture

The application follows a clean architecture with:
- **Models**: Data structures for Employee and Evaluation
- **Services**: Business logic for employee and evaluation management
- **UI**: Console-based user interface
- **In-Memory Storage**: Data is stored in memory during the session

## Future Enhancements

- Persistent storage (database or file system)
- Authentication and role-based access
- Email notifications
- Reporting and analytics
- Web-based interface
