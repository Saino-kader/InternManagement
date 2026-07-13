// Application/Features/Feedbacks/DTOs/FeedbackDto.cs
namespace InterManagement.Application.Features.Feedbacks.DTOs
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }

        // Qui a envoyé : "Mentor" ou "Stagiaire"
        public string SenderType { get; set; } = string.Empty;

        // Nom de l'émetteur affiché dans le tableau Admin
        // → nom du mentor si SenderType = "Mentor"
        // → nom du stagiaire si SenderType = "Stagiaire"
        public string SenderName { get; set; } = string.Empty;

        // Infos relation (nullable car l'un des deux peut être null)
        public int? TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public int? MentorId { get; set; }
        public string MentorName { get; set; } = string.Empty;
    }
}