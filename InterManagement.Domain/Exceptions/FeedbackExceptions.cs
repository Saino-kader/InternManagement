namespace InterManagement.Domain.Exceptions
{
    public class FeedbackNotFoundException : DomainException
    {
        public FeedbackNotFoundException(int id)
            : base($"Le message avec l'identifiant {id} est introuvable")
        { }
    }
}
