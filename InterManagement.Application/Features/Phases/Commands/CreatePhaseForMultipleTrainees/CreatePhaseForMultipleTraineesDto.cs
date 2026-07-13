namespace InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees
{
    public class WeekPlanItemDto
    {
        public int WeekNumber { get; set; }
        public string Course { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    public class CreatePhaseForMultipleTraineesDto
    {
        public int PhaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int MentorId { get; set; }
        public List<int> TraineeIds { get; set; } = [];
        public List<WeekPlanItemDto> Weeks { get; set; } = [];
    }
}
