using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Weeks.DTOs
{
    public class WeekDto
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public PhaseStatus Status { get; set; }
        public int PhaseId { get; set; }
        public string PhaseTitle { get; set; } = string.Empty;
    }
}