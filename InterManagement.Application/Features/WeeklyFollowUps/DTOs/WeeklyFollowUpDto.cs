using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.WeeklyFollowUps.DTOs
{
    public class WeeklyFollowUpDto
    {
        public int Id { get; set; }
        public DateOnly FollowUpDate { get; set; }
        public WeeklyFollowUpStatus Status { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Appreciation { get; set; } = string.Empty;
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public int WeekId { get; set; }
        public string PhaseTitle {get; set;} = string.Empty;
    }
}