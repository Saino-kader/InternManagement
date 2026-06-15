using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class Feedback : BaseModel
    {
        public string Message { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }

        // ── FK 
        public int TraineeId { get; private set; }
        public Trainee Trainee { get; private set; } = null!;

        private Feedback() { }

        public Feedback(
            string message,
            int traineeId)
        {
            // ── Validations ───────────────────
            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Message est obligatoire");

            if (traineeId <= 0)
                throw new DomainException("TraineeId est obligatoire");

            // ── Assignation ───────────────────
            Message   = message;
            TraineeId = traineeId;
            SentAt    = DateTime.UtcNow;
        }

        // ── Méthode métier ────────────────────
        public void UpdateMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Message est obligatoire");

            Message   = message;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}