using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.CompleteFollowUp
{
    public class CompleteFollowUpHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;

        public CompleteFollowUpHandler(
            IWeeklyFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CompleteFollowUpCommand command)
        {
            var followUp = await _repository
                .GetByIdAsync(command.Id);
            if (followUp == null)
                throw new WeeklyFollowUpNotFoundException(command.Id);

            if (followUp.Status == WeeklyFollowUpStatus.Validated)
                throw new WeeklyFollowUpAlreadyDoneException(command.Id);

            // utilise méthode métier du Domain
            followUp.Complete(command.Comment);
            await _repository.UpdateAsync(followUp);
        }
    }
}
