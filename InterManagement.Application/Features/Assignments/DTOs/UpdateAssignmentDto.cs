namespace InterManagement.Application.Features.Assignments.DTOs
{
    public class UpdateAssignmentDto
    {
        public int TraineeId { get; set; }
        public int MentorId { get; set; }
        public int PhaseId { get; set; }
        public bool IsActive { get; set; }
    }
}
