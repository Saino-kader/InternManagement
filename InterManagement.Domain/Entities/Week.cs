using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class Week : BaseModel
    {
        public int WeekNumber { get; private set; }
        public string Course { get; private set; } = string.Empty;
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public PhaseStatus Status { get; private set; }

        // ── FK
        public int PhaseId { get; private set; }
        public Phase Phase { get; private set; } = null!;

        public ICollection<WeeklyFollowUp> WeeklyFollowUps { get; private set; }= new List<WeeklyFollowUp>();

        private Week() { }

        public Week(
            int weekNumber,
            string course,
            DateOnly startDate,
            DateOnly endDate,
            int phaseId)
        {
            // ── Validations
            if (weekNumber <= 0)
                throw new DomainException("Le numéro de semaine doit être supérieur à 0");

            if (string.IsNullOrWhiteSpace(course))
                throw new DomainException("Le cours est obligatoire");

            if (endDate <= startDate)
                throw new DomainException("La date de fin doit être après la date de début");

            if (phaseId <= 0)
                throw new DomainException("La phase est obligatoire");

            // ── Assignation
            WeekNumber = weekNumber;
            Course     = course;
            StartDate  = startDate;
            EndDate    = endDate;
            PhaseId    = phaseId;
            Status     = PhaseStatus.InProgress;
        }

        // ── Méthode Update ────────────────────
        // Chaque semaine a son propre statut, indépendant des autres
        // semaines de la même phase (voir aussi Phase.Status, qui reste
        // un statut distinct au niveau de la phase entière).
        public void Update(
            string course,
            DateOnly startDate,
            DateOnly endDate,
            PhaseStatus status)
        {
            if (string.IsNullOrWhiteSpace(course))
                throw new DomainException("Le cours est obligatoire");

            if (endDate <= startDate)
                throw new DomainException(
                    "La date de fin doit être après la date de début");

            Course    = course;
            StartDate = startDate;
            EndDate   = endDate;
            Status    = status;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}