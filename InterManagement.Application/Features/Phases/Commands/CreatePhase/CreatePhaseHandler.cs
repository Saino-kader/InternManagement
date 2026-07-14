using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Phases.Commands.CreatePhase
{
    public class CreatePhaseHandler
    {
        private readonly IPhaseRepository _repository;
        private readonly ITraineeRepository _traineeRepository;
        public CreatePhaseHandler(
            IPhaseRepository phaseRepository,
            ITraineeRepository traineeRepository)
        {
            _repository        = phaseRepository;
            _traineeRepository = traineeRepository;
        }

        public async Task<PhaseDto> Handle(CreatePhaseCommand command)
        {
            // 1. Vérifier que le Trainee existe
            var trainee = await _traineeRepository
                .GetByIdAsync(command.Data.TraineeId);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Data.TraineeId);

            // 2. Créer la Phase (sans MentorId — géré par Assignment)
            var phase = new Phase(
                command.Data.PhaseNumber,
                command.Data.Title,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.TraineeId
            );

            // 3. Sauvegarder
            await _repository.AddAsync(phase);

            // 4. Retourner DTO
            return new PhaseDto
            {
                Id          = phase.Id,
                PhaseNumber = phase.PhaseNumber,
                Title       = phase.Title,
                StartDate   = phase.StartDate,
                EndDate     = phase.EndDate,
                Status      = phase.Status,
                TraineeId   = phase.TraineeId
            };
        }
    }
}
