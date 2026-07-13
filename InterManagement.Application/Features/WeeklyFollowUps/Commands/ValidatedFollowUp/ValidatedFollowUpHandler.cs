using InterManagement.Application.Common;  // ← Ajoute cette ligne
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using InterManagement.Shared.Enums;  // ← Pour WeeklyFollowUpStatus
using InterManagement.Domain.Entities;  // ← Pour WeeklyFollowUp

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.ValidatedFollowUp
{
    public class ValidatedFollowUpHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;
        private readonly IActivityLogger _activityLogger;

        public ValidatedFollowUpHandler(
            IWeeklyFollowUpRepository repository,
            IActivityLogger activityLogger)
        {
            _repository = repository;
            _activityLogger = activityLogger;
        }

        public async Task Handle(ValidatedFollowUpCommand command)
        {
            var followUp = await _repository.GetByIdAsync(command.Id);
            if (followUp == null)
                throw new WeeklyFollowUpNotFoundException(command.Id);

            if (followUp.Status == WeeklyFollowUpStatus.Validated)
                throw new WeeklyFollowUpAlreadyDoneException(command.Id);

            followUp.Validated();
            await _repository.UpdateAsync(followUp);

            await _activityLogger.LogAsync(
                followUp.Mentor.FirstName + " " + followUp.Mentor.LastName,
                "Suivi validé",
                $"Semaine {followUp.Week.WeekNumber} validée pour " +
                $"{followUp.Trainee.FirstName} {followUp.Trainee.LastName}"
            );
        }
    }
}
