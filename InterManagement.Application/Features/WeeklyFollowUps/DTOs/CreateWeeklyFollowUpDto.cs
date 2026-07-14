using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.WeeklyFollowUps.DTOs
{
    public class CreateWeeklyFollowUpDto
    {
        public int WeekId { get; set; }              
        public DateOnly FollowUpDate { get; set; }
        public string CourseName { get; set; } = string.Empty; 
        public string Appreciation { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public WeeklyFollowUpStatus Status { get; set; }       
        public int TraineeId { get; set; }
        public int MentorId { get; set; }


    }
}
