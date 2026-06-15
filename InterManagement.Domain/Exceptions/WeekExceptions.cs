namespace InterManagement.Domain.Exceptions
{
    public class WeekNotFoundException : DomainException
    {
        public WeekNotFoundException(int id)
            : base($"Week with Id {id} was not found")
        { }
    }

    public class WeekAlreadyExistsException : DomainException
    {
        public WeekAlreadyExistsException(int phaseId, int weekNumber)
            : base($"Week {weekNumber} already exists " +
                   $"for Phase {phaseId}")
        { }
    }
}