namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ValidatedFollowUp
{
    public class ValidatedFollowUpCommand
    {
        public int Id { get; set; }

        public ValidatedFollowUpCommand(int id)
        {
            Id = id;
        }
    }
}
