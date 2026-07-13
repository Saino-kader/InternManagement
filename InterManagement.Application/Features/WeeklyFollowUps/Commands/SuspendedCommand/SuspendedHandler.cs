using InterManagement.Application.Common;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.SuspendedCommand
{
    public class SuspendedHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;
        private readonly IActivityLogger _activityLogger;

        public SuspendedHandler(
            IWeeklyFollowUpRepository repository,
            IActivityLogger activityLogger)
        {
            _repository = repository;
            _activityLogger = activityLogger;
        }

        public async Task Handle(SuspendedCommand command)
        {
            var followUp = await _repository.GetByIdAsync(command.Id);
            if (followUp == null)
                throw new WeeklyFollowUpNotFoundException(command.Id);

            followUp.Suspended();
            await _repository.UpdateAsync(followUp);

            await _activityLogger.LogAsync(
                followUp.Mentor.FirstName + " " + followUp.Mentor.LastName,
                "Suivi suspendu",
                $"Semaine {followUp.Week.WeekNumber} marquée suspendue pour " +
                $"{followUp.Trainee.FirstName} {followUp.Trainee.LastName}"
            );
        }
    }
}
