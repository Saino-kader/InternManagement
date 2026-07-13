// Domain/Entities/Feedback.cs
using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class Feedback : BaseModel
    {
        public string Message { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }

        // TraineeId est nullable :
        // → null quand c'est un message Mentor vers Admin
        // → valeur quand c'est un message Stagiaire
        public int? TraineeId { get; private set; }
        public Trainee? Trainee { get; private set; }

        // MentorId est nullable :
        // → null quand c'est un message Stagiaire
        // → valeur quand c'est un message Mentor
        public int? MentorId { get; private set; }
        public Mentor? Mentor { get; private set; }

        private Feedback() { }

        public Feedback(
            string message,
            int? traineeId = null,
            int? mentorId = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Le message est obligatoire");

            // Au moins un des deux doit être renseigné
            if (traineeId == null && mentorId == null)
                throw new DomainException(
                    "TraineeId ou MentorId est obligatoire");

            Message   = message;
            TraineeId = traineeId;
            MentorId  = mentorId;
            SentAt    = DateTime.UtcNow;
        }

        public void UpdateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Le message est obligatoire");

            Message   = message;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}