using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.InternFiles.Commands.SetFilePath
{
    public class SetFilePathHandler
    {
        private readonly IInternFileRepository _repository;

        public SetFilePathHandler(IInternFileRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(SetFilePathCommand command)
        {
            var file = await _repository.GetByIdAsync(command.Id);
            if (file == null)
                throw new InternFileNotFoundException(command.Id);

            // utilise SetFilePath du Domain
            file.SetFilePath(command.FilePath);
            await _repository.UpdateAsync(file);
        }
    }
}