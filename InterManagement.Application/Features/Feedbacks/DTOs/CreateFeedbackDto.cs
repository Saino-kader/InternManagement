// Application/Features/Feedbacks/DTOs/CreateFeedbackDto.cs
namespace InterManagement.Application.Features.Feedbacks.DTOs
{
    public class CreateFeedbackDto
    {
        public string Message { get; set; } = string.Empty;

        // Nullable : null quand message vient d'un Mentor
        public int? TraineeId { get; set; }

        // Nullable : null quand message vient d'un Stagiaire
        public int? MentorId { get; set; }
    }
}