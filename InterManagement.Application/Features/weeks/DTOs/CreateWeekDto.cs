namespace InterManagement.Application.Features.Weeks.DTOs
{
    public class CreateWeekDto
    {
        public int WeekNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PhaseId { get; set; }
    }
}