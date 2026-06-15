using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;



namespace InterManagement.Domain.Entities
{

    public class Mentor : User
    {
        public string Department { get; private set; } = string.Empty;
        public string Specialty { get; private set; } = string.Empty;

        // ── Collections de relations ────────────────────
        public ICollection<Assignment> Assignments { get; private set; } = [];
        public ICollection<WeeklyFollowUp> WeeklyFollowUps { get; private set; } = [];

        private Mentor() { }

        public Mentor(
            string firstName,
            string lastName,
            string email,
            string department,
            string specialty)
            : base(firstName, lastName, email, UserRole.Mentor)
        {
            // ── Champs obligatoires ────────────────
            if (string.IsNullOrWhiteSpace(department))
                throw new DomainException("Le département est obligatoire");  

            if (string.IsNullOrWhiteSpace(specialty))
                throw new DomainException("La spécialité est obligatoire"); 

            // ── Assignation ────────────────────────
            Department = department;
            Specialty  = specialty;
        }

        // ── Méthode Update ────────────────────────
        public void Update(
            string firstName,
            string lastName,
            string email,
            string department,
            string specialty)
        {
           // if (string.IsNullOrWhiteSpace(firstName))
                //throw new DomainException("Le prénom est obligatoire"); // est ce que c'est obligatoire de verifier les champs lors d'une modification

            //if (string.IsNullOrWhiteSpace(department))
                //throw new DomainException("Le département est obligatoire");
            
            //if (string.IsNullOrWhiteSpace(specialty))
                //throw new DomainException("La spécialité est obligatoire");

            FirstName  = firstName;
            LastName   = lastName;
            Email      = email;
            Department = department;
            Specialty  = specialty;
            UpdatedAt  = DateTime.UtcNow;
        }
    }

}