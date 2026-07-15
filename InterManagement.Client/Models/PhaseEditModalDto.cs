using InterManagement.Shared.Enums;

namespace InterManagement.Client.Models;

public class PhaseEditModalDto
{
    public int PhaseId { get; set; }
    public int WeekId { get; set; }
    public PhaseStatus Status { get; set; }
    public PhaseStatus WeekStatus { get; set; }
    public string Course { get; set; } = string.Empty;
    public DateOnly WeekStartDate { get; set; }
    public DateOnly WeekEndDate { get; set; }
}
