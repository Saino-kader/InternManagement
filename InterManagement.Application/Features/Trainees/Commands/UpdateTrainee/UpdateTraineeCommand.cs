
using InterManagement.Application.Features.Trainees.DTOs;

namespace InterManagement.Application.Features.Trainees.Commands.UpdateTrainee
{
    public class UpdateTraineeCommand
    {
        public int Id { get; set; }
        public UpdateTraineeDto Data { get; set; }

        public UpdateTraineeCommand(int id, UpdateTraineeDto data)
        {
            Id   = id;
            Data = data;
        }
    }
}
