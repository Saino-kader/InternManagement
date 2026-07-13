namespace InterManagement.Client.Models;

public class MentorAssignedTraineeItem
{
    public int TraineeId { get; set; }
    public string TraineeName { get; set; } = string.Empty;
    public string PhaseTitle { get; set; } = string.Empty;
    public string PhaseNumber { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateOnly? WeekStartDate { get; set; }
    public DateOnly? WeekEndDate { get; set; }
    public string StatusText { get; set; } = "InProgress";
}