using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class Admin : User
    {
        private Admin() { }

        public Admin(
            string firstName,
            string lastName,
            string email)
            : base(firstName, lastName, email, UserRole.Admin)
        {
            // pas de champs supplémentaires
            // User vérifie déjà firstName, lastName, email
        }

        // ── Méthode Update ────────────────────
        public void Update(
            string firstName,
            string lastName,
            string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Le nom est obligatoire");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Le Prénom est obligatoire");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("L'email est obligatoire");

            if (!email.Contains("@") || !email.Contains("."))
                throw new DomainException("Le format de l'email est invalide");

            FirstName = firstName;
            LastName  = lastName;
            Email     = email;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

