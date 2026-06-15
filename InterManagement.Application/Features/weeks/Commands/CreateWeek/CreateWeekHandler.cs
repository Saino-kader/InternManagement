using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Commands.CreateWeek
{
    public class CreateWeekHandler
    {
        private readonly IWeekRepository  _repository;
        private readonly IPhaseRepository _phaseRepository;

        public CreateWeekHandler(
            IWeekRepository  weekRepository,
            IPhaseRepository phaseRepository)
        {
            _repository      = weekRepository;
            _phaseRepository = phaseRepository;
        }

        public async Task<WeekDto> Handle(CreateWeekCommand command)
        {
            // 1. Vérifier Phase existe
            var phase = await _phaseRepository
                .GetByIdAsync(command.Data.PhaseId);
            if (phase == null)
                throw new PhaseNotFoundException(
                    command.Data.PhaseId);

            // 2. Vérifier semaine pas déjà existante
            var exists = await _repository.WeekExistsAsync(
                command.Data.PhaseId,
                command.Data.WeekNumber);
            if (exists)
                throw new WeekAlreadyExistsException(
                    command.Data.PhaseId,
                    command.Data.WeekNumber);

            // 3. Créer la semaine
            var week = new Week(
                command.Data.WeekNumber,
                command.Data.Course,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.PhaseId
            );

            // 4. Sauvegarder
            await _repository.AddAsync(week);

            // 5. Retourner DTO
            return new WeekDto
            {
                Id         = week.Id,
                WeekNumber = week.WeekNumber,
                Course     = week.Course,
                StartDate  = week.StartDate,
                EndDate    = week.EndDate,
                PhaseId    = week.PhaseId,
                PhaseTitle = phase.Title
            };
        }
    }
}
