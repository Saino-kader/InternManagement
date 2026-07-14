namespace InterManagement.Domain.Exceptions
{
    public class WeeklyFollowUpNotFoundException : DomainException
    {
        public WeeklyFollowUpNotFoundException(int id)
            : base($"Le suivi hebdomadaire avec l'identifiant {id} est introuvable")
        { }
    }

    public class WeeklyFollowUpAlreadyExistsException : DomainException
    {
        public WeeklyFollowUpAlreadyExistsException(
            int traineeId, int weekId)
            : base($"Un suivi existe déjà pour le stagiaire {traineeId}, " +
                   $"semaine {weekId}")
        { }
    }

    public class WeeklyFollowUpAlreadyDoneException : DomainException
    {
        public WeeklyFollowUpAlreadyDoneException(int id)
            : base($"Le suivi hebdomadaire avec l'identifiant {id} est déjà terminé")
        { }
    }
}
