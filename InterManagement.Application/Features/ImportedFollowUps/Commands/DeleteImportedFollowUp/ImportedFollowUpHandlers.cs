using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.ImportedFollowUps.Commands.DeleteImportedFollowUp
{
    public class DeleteImportedFollowUpCommand
    {
        public int Id { get; set; }
        public DeleteImportedFollowUpCommand(int id) => Id = id;
    }

    public class DeleteImportedFollowUpHandler
    {
        private readonly IImportedFollowUpRepository _repository;

        public DeleteImportedFollowUpHandler(IImportedFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteImportedFollowUpCommand command)
        {
            await _repository.DeleteAsync(command.Id);
        }
    }
}
