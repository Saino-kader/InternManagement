using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class Phase : BaseModel
    {
        public int PhaseNumber { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public PhaseStatus Status { get; private set; }

        // ── FK 
        public int TraineeId { get; private set; }
        public Trainee Trainee { get; private set; } = null!;

        // ── Collections 
        public ICollection<WeeklyFollowUp> WeeklyFollowUps { get; private set; } = [];
        public ICollection<Assignment> Assignments { get; private set; } = [];

        public ICollection<Week> Weeks { get; private set; } = [];

        private Phase() { }

        public Phase(
            int phaseNumber,
            string title,
            DateOnly startDate,
            DateOnly endDate,
            int traineeId)
        {
            // ── Validations 
            if (phaseNumber <= 0)
                throw new DomainException("Le numéro de phase doit être supérieur à 0");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Le titre est obligatoire");

            if (endDate <= startDate)
                throw new DomainException("La date de fin doit être après la date de début");

            if (traineeId <= 0)
                throw new DomainException("Le stagiaire est obligatoire");

            // ── Assignation 
            PhaseNumber = phaseNumber;
            Title       = title;
            StartDate   = startDate;
            EndDate     = endDate;
            TraineeId   = traineeId;
            Status      = PhaseStatus.InProgress;
        }

        // ── Méthode Update 
        public void Update(
            string title,
            DateOnly startDate,
            DateOnly endDate,
            PhaseStatus status)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Le titre est obligatoire");

            if (endDate <= startDate)
                throw new DomainException("La date de fin doit être après la date de début");

            Title     = title;
            StartDate = startDate;
            EndDate   = endDate;
            Status    = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}

