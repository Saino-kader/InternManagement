using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Trainees.Commands.UpdateTrainee
{
    public class UpdateTraineeHandler
    {
        private readonly ITraineeRepository _repository;

        public UpdateTraineeHandler(ITraineeRepository repository)
        {
            _repository = repository;
        }

        public async Task<TraineeDto> Handle(UpdateTraineeCommand command)
        {
            // 1. Chercher le stagiaire
            var trainee = await _repository.GetByIdAsync(command.Id);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Id);

            // 2. Vérifier qu'il est actif
            if (!trainee.IsActive)
                throw new TraineeNotActiveException(command.Id);

            // 3. Modifier les champs
            trainee.Update(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email,
                command.Data.University,
                command.Data.Specialty,
                command.Data.Theme,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.Status
            );

            
            // 4. Sauvegarder
            await _repository.UpdateAsync(trainee);

            // 5. Retourner le DTO
            return new TraineeDto
            {
                Id         = trainee.Id,
                FirstName  = trainee.FirstName,
                LastName   = trainee.LastName,
                Email      = trainee.Email,
                University = trainee.University,
                Specialty  = trainee.Specialty,
                Theme      = trainee.Theme,
                StartDate  = trainee.StartDate,
                EndDate    = trainee.EndDate,
                Status     = trainee.Status,
                IsActive   = trainee.IsActive
            };
        }
    }
}





































