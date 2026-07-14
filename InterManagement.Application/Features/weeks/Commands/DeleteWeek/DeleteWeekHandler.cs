using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Commands.DeleteWeek
{
    public class DeleteWeekHandler
    {
        private readonly IWeekRepository _repository;

        public DeleteWeekHandler(IWeekRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteWeekCommand command)
        {
            var week = await _repository.GetByIdAsync(command.Id);
            if (week == null)
                throw new WeekNotFoundException(command.Id);

            await _repository.DeleteAsync(command.Id);
        }
    }
}
