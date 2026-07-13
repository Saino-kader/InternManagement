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

            // utilise Update() pour mettre à jour tous les champs
            followUp.Update(
                command.Data.FollowUpDate,
                command.Data.Comment,
                command.Data.WeekId,
                command.Data.TraineeId,
                command.Data.MentorId,
                command.Data.CourseName,
                command.Data.Appreciation
                );

            followUp.SetStatus(command.Data.Status);

            await _repository.UpdateAsync(followUp);
        }
    }
}
