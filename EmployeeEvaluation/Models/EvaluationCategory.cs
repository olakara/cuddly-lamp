namespace EmployeeEvaluation.Models;

public enum EvaluationCategory
{
    Attendance,
    WorkQuality,
    Innovation,
    Communication,
    SkillDevelopment
}

public static class EvaluationCategoryExtensions
{
    public static string GetDisplayName(this EvaluationCategory category)
    {
        return category switch
        {
            EvaluationCategory.Attendance => "Attendance",
            EvaluationCategory.WorkQuality => "Work Quality",
            EvaluationCategory.Innovation => "Innovation",
            EvaluationCategory.Communication => "Communication",
            EvaluationCategory.SkillDevelopment => "Skill Development",
            _ => category.ToString()
        };
    }
}