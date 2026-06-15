namespace InterManagement.Application.Features.WeeklyFollowUps.DTOs
{
    public class UpdateWeeklyFollowUpDto
    {
        public DateOnly FollowUpDate { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}