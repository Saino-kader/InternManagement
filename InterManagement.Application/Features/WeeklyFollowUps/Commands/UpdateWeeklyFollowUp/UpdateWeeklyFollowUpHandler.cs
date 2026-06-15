using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp
{
    public class UpdateWeeklyFollowUpHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;

        public UpdateWeeklyFollowUpHandler(IWeeklyFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateWeeklyFollowUpCommand command)
        {
            var followUp = await _repository.GetByIdAsync(command.Id);
            if (followUp == null)
                throw new WeeklyFollowUpNotFoundException(command.Id);

            // utilise UpdateComment() déjà existant dans Domain
            followUp.UpdateComment(command.Data.Comment);

            await _repository.UpdateAsync(followUp);
        }
    }
}