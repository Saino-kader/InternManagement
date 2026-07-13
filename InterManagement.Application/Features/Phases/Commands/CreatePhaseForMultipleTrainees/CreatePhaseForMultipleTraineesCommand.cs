namespace InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees
{
    public class CreatePhaseForMultipleTraineesCommand
    {
        public CreatePhaseForMultipleTraineesDto Data { get; set; }

        public CreatePhaseForMultipleTraineesCommand(CreatePhaseForMultipleTraineesDto data)
        {
            Data = data;
        }
    }
}