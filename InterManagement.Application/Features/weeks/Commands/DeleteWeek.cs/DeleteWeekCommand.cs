namespace InterManagement.Application.Features.Weeks.Commands.DeleteWeek
{
    public class DeleteWeekCommand
    {
        public int Id { get; set; }

        public DeleteWeekCommand(int id)
        {
            Id = id;
        }
    }
}