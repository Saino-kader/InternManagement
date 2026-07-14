using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class WeeklyFollowUp : BaseModel
    {
        public DateOnly FollowUpDate { get; private set; }
        public string CourseName { get; private set; } = string.Empty;
        public string Appreciation { get; private set; } = string.Empty;
        public WeeklyFollowUpStatus Status { get; private set; }
        public string Comment { get; private set; } = string.Empty;

        public int TraineeId { get; private set; }
        public Trainee Trainee { get; private set; } = null!;

        public int MentorId { get; private set; }
        public Mentor Mentor { get; private set; } = null!;

        public int WeekId { get; private set; }
        public Week Week { get; private set; } = null!;

        private WeeklyFollowUp() { }

        public WeeklyFollowUp(
            DateOnly followUpDate,
            string comment,
            int traineeId,
            int mentorId,
            int weekId,
            string courseName,
            string appreciation)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Le commentaire est obligatoire");

            if (traineeId <= 0)
                throw new DomainException("Le stagiaire est obligatoire");

            if (mentorId <= 0)
                throw new DomainException("Le Mentor est obligatoire");

            if (weekId <= 0)
                throw new DomainException("La semaine est obligatoire");

            if (string.IsNullOrWhiteSpace(courseName))
                throw new DomainException("Le nom du cours est obligatoire");

            FollowUpDate = followUpDate;
            Comment      = comment;
            TraineeId    = traineeId;
            MentorId     = mentorId;
            WeekId       = weekId;
            CourseName   = courseName;
            Appreciation = appreciation;
            Status       = WeeklyFollowUpStatus.InProgress;
        }

        public void Validated()
        {
            
            if (Status == WeeklyFollowUpStatus.Validated)
                throw new DomainException("Le suivi est déjà validé");
        
            if (Status == WeeklyFollowUpStatus.Suspended)
            throw new DomainException("Un stage suspendu ne peut pas être terminé");

            Status    = WeeklyFollowUpStatus.Validated;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Suspended()
        {
            if (Status == WeeklyFollowUpStatus.Suspended)
                throw new DomainException("Le suivi est déjà suspendu");

            if (Status == WeeklyFollowUpStatus.Validated)
                throw new DomainException("Un suivi validé ne peut pas être suspendu");

            Status    = WeeklyFollowUpStatus.Suspended;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Commentaire est obligatoire");

            Comment   = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(
            DateOnly followUpDate,
            string comment,
            int weekId,
            int traineeId,
            int mentorId,
            string courseName,
            string appreciation)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new DomainException("Commentaire est obligatoire");

            if (weekId <= 0)
                throw new DomainException("La semaine est obligatoire");

            if (string.IsNullOrWhiteSpace(courseName))
                throw new DomainException("Le nom du cours est obligatoire");

            FollowUpDate = followUpDate;
            Comment      = comment;
            WeekId       = weekId;
            CourseName   = courseName;
            Appreciation = appreciation;
            TraineeId    = traineeId;
            MentorId     = mentorId;
            UpdatedAt    = DateTime.UtcNow;
        }

        public void SetStatus(WeeklyFollowUpStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

