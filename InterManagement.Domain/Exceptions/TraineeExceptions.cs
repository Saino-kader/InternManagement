namespace InterManagement.Domain.Exceptions
{
    // ── Introuvable ───────────────────────────────
    public class TraineeNotFoundException : DomainException
    {
        public TraineeNotFoundException(int id)
            : base($"Le stagiaire avec l'identifiant {id} est introuvable")
        { }
    }

    // ── Déjà existant ─────────────────────────────
    public class TraineeAlreadyExistsException : DomainException
    {
        public TraineeAlreadyExistsException(string email)
            : base($"Un stagiaire avec l'email {email} existe déjà")
        { }
    }

    // ── Pas actif ─────────────────────────────────
    public class TraineeNotActiveException : DomainException
    {
        public TraineeNotActiveException(int id)
            : base($"Le stagiaire avec l'identifiant {id} n'est pas actif")
        { }
    }

    // ── Déjà assigné ───────────────────────────────
    public class TraineeAlreadyAssignedException : DomainException
    {
        public TraineeAlreadyAssignedException(int id)
            : base($"Le stagiaire avec l'identifiant {id} a déjà un mentor assigné actif")
        { }
    }
}
