using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Phases.Commands.UpdatePhase
{
    public class UpdatePhaseHandler
    {
        private readonly IPhaseRepository _repository;

        public UpdatePhaseHandler(IPhaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PhaseDto> Handle(UpdatePhaseCommand command)
        {
            // 1. Chercher la phase
            var phase = await _repository.GetByIdAsync(command.Id);
            if (phase == null)
                throw new PhaseNotFoundException(command.Id);

            // 2. Vérifier statut
            if (phase.Status == PhaseStatus.Validated)
                throw new PhaseAlreadyCompletedException(command.Id);

            if (phase.Status == PhaseStatus.Suspended)
                throw new PhaseCancelledException(command.Id);

            // 3. Modifier
            phase.Update(
                command.Data.Title,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.Status
            );

            // 4. Sauvegarder
            await _repository.UpdateAsync(phase);

            // 5. Retourner DTO
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
