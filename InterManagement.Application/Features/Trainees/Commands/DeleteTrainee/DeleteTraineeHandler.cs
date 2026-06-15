using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Trainees.Commands.DeleteTrainee
{
    public class DeleteTraineeHandler
    {
        private readonly ITraineeRepository _repository;

        public DeleteTraineeHandler(ITraineeRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteTraineeCommand command)
        {
            // 1. Chercher le stagiaire
            var trainee = await _repository.GetByIdAsync(command.Id);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Id);

            // 2. Supprimer (soft delete)
            await _repository.DeleteAsync(command.Id);
        }
    }
}































































