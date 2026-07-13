namespace InterManagement.Application.Features.Phases.DTOs
{
    public class CreatePhaseDto
    {
        public int PhaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TraineeId { get; set; }
    }
}
