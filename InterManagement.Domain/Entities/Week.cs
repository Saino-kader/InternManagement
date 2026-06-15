using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class Week : BaseModel
    {
        public int WeekNumber { get; private set; }
        public string Course { get; private set; } = string.Empty;
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }

        // ── FK 
        public int PhaseId { get; private set; }
        public Phase Phase { get; private set; } = null!;

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
                throw new InterManagement.Domain.Exceptions.DomainException("Numero must be greater than 0");

            if (string.IsNullOrWhiteSpace(course))
                throw new InterManagement.Domain.Exceptions.DomainException("Course is required");

            if (endDate <= startDate)
                throw new InterManagement.Domain.Exceptions.DomainException("End date must be after start date");

            if (phaseId <= 0)
                throw new InterManagement.Domain.Exceptions.DomainException("PhaseId is required");

            // ── Assignation 
            WeekNumber = weekNumber;
            Course     = course;
            StartDate  = startDate;
            EndDate    = endDate;
            PhaseId    = phaseId;
        }

        // ── Méthode Update ────────────────────
        public void Update(
            string course,
            DateOnly startDate,
            DateOnly endDate)
        {
            if (string.IsNullOrWhiteSpace(course))
                throw new InterManagement.Domain.Exceptions.DomainException("Course is required");

            if (endDate <= startDate)
                throw new InterManagement.Domain.Exceptions.DomainException(
                    "End date must be after start date");

            Course    = course;
            StartDate = startDate;
            EndDate   = endDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}