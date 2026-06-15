using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Entities
{
    public class User : BaseModel
    {

        public string FirstName { get;  set; } = string.Empty;

        public string LastName { get;  set; } = string.Empty;
        public string Email { get;  set; } = string.Empty;
        public UserRole Role { get;  set; }
        public bool IsActive { get;  set; } = true;

        public User() { }

        public User(string firstName, string lastName, string email, UserRole role)
        {
            
            // ── Champs obligatoires ────────────────────
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("Nom est obligatoire");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Prénom est obligatoire");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email est obligatoire");

            // ── Format email ───────────────────────────
            if (!email.Contains("@") || !email.Contains("."))
                throw new DomainException("Format Email  est invalide");

            // ── Assignation ───────────────────────────

            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            IsActive = true;
        }
    
    }
    
}


