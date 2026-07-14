using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class Trainee : User
    {
        public string University { get; private set; } = string.Empty;
        public string Specialty { get; private set; } = string.Empty;
        public string Theme { get; private set; } = string.Empty;
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public TraineeStatus Status { get; private set; } = TraineeStatus.InProgress;

        // ── Collections de relations ───────────
        public ICollection<Assignment> Assignments { get; private set; } = new List<Assignment>();
        public ICollection<WeeklyFollowUp> WeeklyFollowUps { get; private set; } = new List<WeeklyFollowUp>();
        public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();
        public ICollection<Phase> Phases { get; private set; } = new List<Phase>();

        private Trainee() { }

        public Trainee(
            string firstName,
            string lastName,
            string email,
            string university,
            string specialty,
            string theme,
            DateOnly startDate,
            DateOnly endDate,
            TraineeStatus status)
            : base(firstName, lastName, email, UserRole.Trainee)
        {
            // ── Champs obligatoires ───────────
            if (string.IsNullOrWhiteSpace(university))
                throw new DomainException("L'université est obligatoire");

            if (string.IsNullOrWhiteSpace(specialty))
                throw new DomainException("La spécialité est obligatoire");

            if (string.IsNullOrWhiteSpace(theme))
                throw new DomainException("Le thème est obligatoire");

            // ── Règle métier dates ────────────
            if (endDate <= startDate)
                throw new DomainException("La date de fin doit être après la date de début");

            if (startDate < DateOnly.FromDateTime(DateTime.Today))
                throw new DomainException("La date de début ne peut pas être dans le passé");

            University = university;
            Specialty  = specialty;
            Theme      = theme;
            StartDate  = startDate;
            EndDate    = endDate;
            Status     = TraineeStatus.InProgress;
        }

        public void Update(
            string firstName,
            string lastName,
            string email,
            string university,
            string specialty,
            string theme,
            DateOnly startDate,
            DateOnly endDate,
            TraineeStatus status)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Le prénom est obligatoire");

            if (endDate <= startDate)
                throw new DomainException("La date de fin doit être après la date de début");

            FirstName  = firstName;
            LastName   = lastName;
            Email      = email;
            University = university;
            Specialty  = specialty;
            Theme      = theme;
            StartDate  = startDate;
            EndDate    = endDate;
            Status     = status;
            UpdatedAt  = DateTime.UtcNow;
        }
    }
}
