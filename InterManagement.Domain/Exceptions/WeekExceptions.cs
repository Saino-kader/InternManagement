namespace InterManagement.Domain.Exceptions
{
    public class WeekNotFoundException : DomainException
    {
        public WeekNotFoundException(int id)
            : base($"La semaine avec l'identifiant {id} est introuvable")
        { }
    }

    public class WeekAlreadyExistsException : DomainException
    {
        public WeekAlreadyExistsException(int phaseId, int weekNumber)
            : base($"La semaine {weekNumber} existe déjà pour la phase {phaseId}")
        { }
    }
}
