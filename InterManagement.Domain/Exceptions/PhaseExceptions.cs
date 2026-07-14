namespace InterManagement.Domain.Exceptions
{
    public class PhaseNotFoundException : DomainException
    {
        public PhaseNotFoundException(int id)
            : base($"La phase avec l'identifiant {id} est introuvable")
        { }
    }

    public class PhaseAlreadyCompletedException : DomainException
    {
        public PhaseAlreadyCompletedException(int id)
            : base($"La phase avec l'identifiant {id} est déjà terminée")
        { }
    }

    public class PhaseCancelledException : DomainException
    {
        public PhaseCancelledException(int id)
            : base($"La phase avec l'identifiant {id} est annulée")
        { }
    }
}
