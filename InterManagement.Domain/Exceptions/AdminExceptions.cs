namespace InterManagement.Domain.Exceptions
{
    public class AdminNotFoundException : DomainException
    {
        public AdminNotFoundException(int id)
            : base($"L'administrateur avec l'identifiant {id} est introuvable")
        { }
    }

    public class AdminAlreadyExistsException : DomainException
    {
        public AdminAlreadyExistsException(string email)
            : base($"Un administrateur avec l'email {email} existe déjà")
        { }
    }

    public class AdminNotActiveException : DomainException
    {
        public AdminNotActiveException(int id)
            : base($"L'administrateur avec l'identifiant {id} n'est pas actif")
        { }
    }
}
