
using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class WeeklyFollowUp : BaseModel
    {
        public int WeekNumber { get; private set; }
        public DateOnly FollowUpDate { get; private set; }
        public WeeklyFollowUpStatus Status { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        // ── FKs ───────────────────────────────
        public int PhaseId { get; private set; }
        public Phase Phase { get; private set; } = null!;

        public int TraineeId { get; private set; }
        public Trainee Trainee { get; private set; } = null!;

        public int MentorId { get; private set; }
        public Mentor Mentor { get; private set; } = null!;

        private WeeklyFollowUp() { }

        public WeeklyFollowUp(
            int weekNumber,
            DateOnly followUpDate,
            string comment,
            int phaseId,
            int traineeId,
            int mentorId)
        {
            // ── Validations ───────────────────
            if (weekNumber <= 0)
                throw new DomainException("Week number must be greater than 0");

            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Le commentaire est obligatoire");

            if (phaseId <= 0)
                throw new DomainException(" La Phase est obligatoire");

            if (traineeId <= 0)
                throw new DomainException("Le stagiaire est obligatoire");

            if (mentorId <= 0)
                throw new DomainException("Le Mentor est obligatoire");

            // ── Assignation ───────────────────
            WeekNumber   = weekNumber;
            FollowUpDate = followUpDate;
            Comment      = comment;
            PhaseId      = phaseId;
            TraineeId    = traineeId;
            MentorId     = mentorId;
            Status       = WeeklyFollowUpStatus.InProgress;
        }

        // ── Méthodes métier ───────────────────
        public void Complete(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Comment is required");

            Status    = WeeklyFollowUpStatus.Validated;
            Comment   = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkMissed()
        {
            Status    = WeeklyFollowUpStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Comment is required");

            Comment   = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(DateOnly followUpDate, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Comment is required");

            FollowUpDate = followUpDate;
            Comment      = comment;
            UpdatedAt    = DateTime.UtcNow;
        }
    }
}