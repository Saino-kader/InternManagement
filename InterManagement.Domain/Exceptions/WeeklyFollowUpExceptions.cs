namespace InterManagement.Domain.Exceptions
{
    public class WeeklyFollowUpNotFoundException : DomainException
    {
        public WeeklyFollowUpNotFoundException(int id)
            : base($"Weekly follow-up with Id {id} was not found")
        { }
    }

    public class WeeklyFollowUpAlreadyExistsException : DomainException
    {
        public WeeklyFollowUpAlreadyExistsException(
            int traineeId, int weekId)
            : base($"A follow-up already exists for " +
                   $"Trainee {traineeId}, " +
                   $"Week {weekId}")
        { }
    }

    public class WeeklyFollowUpAlreadyDoneException : DomainException
    {
        public WeeklyFollowUpAlreadyDoneException(int id)
            : base($"Weekly follow-up with Id {id} is already done")
        { }
    }
}
