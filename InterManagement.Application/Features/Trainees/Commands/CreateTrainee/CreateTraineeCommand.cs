using InterManagement.Application.Features.Trainees.DTOs;

namespace InterManagement.Application.Features.Trainees.Commands.CreateTrainee
{

    public class CreateTraineeCommand
    {
        // CreateTraineeDto → le type de la propriété (c'est le DTO)
        public CreateTraineeDto Data { get; set; }
    
    
        // (CreateTraineeDto data) → paramètre : reçoit le DTO du client
        public CreateTraineeCommand(CreateTraineeDto data)    

        {
            Data = data;
        }
    }
}
