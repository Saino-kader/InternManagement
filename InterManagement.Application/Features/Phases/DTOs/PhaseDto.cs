using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Phases.DTOs
{
    public class PhaseDto
    {
        public int Id { get; set; }
        public int PhaseNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public PhaseStatus Status { get; set; }
        public int TraineeId { get; set; }
        public int? MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
    }
}
