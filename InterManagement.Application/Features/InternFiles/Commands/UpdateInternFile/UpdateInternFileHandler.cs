using InterManagement.Application.Features.InternFiles.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.InternFiles.Commands.UpdateInternFile
{
    public class UpdateInternFileHandler
    {
        private readonly IInternFileRepository _repository;

        public UpdateInternFileHandler(IInternFileRepository repository)
        {
            _repository = repository;
        }

        public async Task<InternFileDto> Handle(
            UpdateInternFileCommand command)
        {
            // 1. Chercher le fichier
            var file = await _repository.GetByIdAsync(command.Id);
            if (file == null)
                throw new InternFileNotFoundException(command.Id);

            // 2. Modifier le nom
            // UpdateFilePath n'est PAS utilisé ici
            // car FilePath est généré par le serveur
            // pas modifiable manuellement
            file.UpdateFileName(command.Data.FileName);

            // 3. Sauvegarder
            await _repository.UpdateAsync(file);

            // 4. Retourner DTO
            return new InternFileDto
            {
                Id          = file.Id,
                FileName    = file.FileName,
                FilePath    = file.FilePath,
                FileType    = file.FileType,
                ImportedAt  = file.ImportedAt,
                TraineeId   = file.TraineeId,
                TraineeName = $"{file.Trainee.FirstName} {file.Trainee.LastName}"
            };
        }
    }
}
