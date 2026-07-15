using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Commands.UpdateWeek
{
    public class UpdateWeekHandler
    {
        private readonly IWeekRepository _repository;

        public UpdateWeekHandler(IWeekRepository repository)
        {
            _repository = repository;
        }

        public async Task<WeekDto> Handle(UpdateWeekCommand command)
        {
            // 1. Chercher la semaine
            var week = await _repository.GetByIdAsync(command.Id);
            if (week == null)
                throw new WeekNotFoundException(command.Id);

            // 2. Modifier via méthode Domain
            week.Update(
                command.Data.Course,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.Status
            );

            // 3. Sauvegarder
            await _repository.UpdateAsync(week);

            // 4. Retourner DTO
            return new WeekDto
            {
                Id         = week.Id,
                WeekNumber = week.WeekNumber,
                Course     = week.Course,
                StartDate  = week.StartDate,
                EndDate    = week.EndDate,
                Status     = week.Status,
                PhaseId    = week.PhaseId,
                PhaseTitle = week.Phase.Title
            };
        }
    }
}