
namespace InterManagement.Application.Features.Trainees.Commands.DeleteTrainee
{
    public class DeleteTraineeCommand
    {
        public int Id { get; set; }

        public DeleteTraineeCommand(int id)
        {
            Id = id;
        }
    }
}
