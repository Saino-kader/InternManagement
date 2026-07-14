namespace InterManagement.Application.Features.Assignments.DTOs
{
    public class MentorAssignmentDetailDto
    {
        public int Id { get; set; }
        public DateTime AssignmentDate { get; set; }
        public bool IsActive { get; set; }

        // infos des relations
        public int TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;

        public int MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;

        public int PhaseId { get; set; }
        public string PhaseTitle { get; set; } = string.Empty;
        public int? PhaseNumber { get; set; }
        public string PhaseStatus { get; set; } = string.Empty;

        // Weeks de la phase
        public List<MentorWeekItemDto> Weeks { get; set; } = new();
    }

    public class MentorWeekItemDto
    {
        public int Id { get; set; }
        public int WeekNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        
        // WeeklyFollowUp pour ce stagiaire et cette semaine
        public string? Status { get; set; }
    }
}
