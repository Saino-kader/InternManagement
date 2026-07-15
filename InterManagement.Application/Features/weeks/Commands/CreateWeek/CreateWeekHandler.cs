// Application/Features/Weeks/Commands/CreateWeek/CreateWeekHandler.cs
using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Commands.CreateWeek
{
    public class CreateWeekHandler
    {
        private readonly IWeekRepository  _weekRepository;
        private readonly IPhaseRepository _phaseRepository;

        public CreateWeekHandler(
            IWeekRepository  weekRepository,
            IPhaseRepository phaseRepository)
        {
            _weekRepository  = weekRepository;
            _phaseRepository = phaseRepository;
        }

        public async Task<WeekDto> Handle(CreateWeekCommand command)
        {
            // 1. Vérifier que la phase existe
            var phase = await _phaseRepository.GetByIdAsync(command.Data.PhaseId);
            if (phase == null)
                throw new PhaseNotFoundException(command.Data.PhaseId);

            // 2. Vérifier l'unicité du numéro de semaine dans cette phase
            //    WeekExistsAsync(phaseId, weekNumber) existe déjà dans WeekRepository ✅
            var exists = await _weekRepository.WeekExistsAsync(
                command.Data.PhaseId,
                command.Data.WeekNumber);

            if (exists)
                throw new WeekAlreadyExistsException(
                    command.Data.PhaseId,
                    command.Data.WeekNumber);

            // 3. Vérifications métier
            if (command.Data.WeekNumber <= 0)
                throw new DomainException("Le numéro de semaine doit être supérieur à 0");

            if (command.Data.EndDate <= command.Data.StartDate)
                throw new DomainException("La date de fin doit être après la date de début");

            // 4. Créer la semaine
            var week = new Week(
                command.Data.WeekNumber,
                command.Data.Course,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.PhaseId
            );

            await _weekRepository.AddAsync(week);

            return new WeekDto
            {
                Id         = week.Id,
                WeekNumber = week.WeekNumber,
                Course     = week.Course,
                StartDate  = week.StartDate,
                EndDate    = week.EndDate,
                Status     = week.Status,
                PhaseId    = week.PhaseId,
                PhaseTitle = phase.Title
            };
        }
    }
}