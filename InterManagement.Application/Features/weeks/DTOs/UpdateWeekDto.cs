using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Weeks.DTOs
{
    public class UpdateWeekDto
    {
        public string Course { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public PhaseStatus Status { get; set; }
    }
}