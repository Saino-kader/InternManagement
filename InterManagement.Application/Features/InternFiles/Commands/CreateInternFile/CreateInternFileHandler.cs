using InterManagement.Application.Features.InternFiles.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.InternFiles.Commands.CreateInternFile
{
    public class CreateInternFileHandler
    {
        private readonly IInternFileRepository _repository;
        private readonly ITraineeRepository    _traineeRepository;

        public CreateInternFileHandler(
            IInternFileRepository internFileRepository,
            ITraineeRepository    traineeRepository)
        {
            _repository        = internFileRepository;
            _traineeRepository = traineeRepository;
        }


        public async Task<InternFileDto> Handle(
            CreateInternFileCommand command)
        {
            // 1. Vérifier Trainee existe
            var trainee = await _traineeRepository
                .GetByIdAsync(command.Data.TraineeId);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Data.TraineeId);

            // 2. Vérifier fichier pas déjà existant
            var exists = await _repository.FileExistsAsync(
                command.Data.TraineeId,
                command.Data.FileName);
            if (exists)
                throw new InternFileAlreadyExistsException(
                    command.Data.FileName);

            // 3. Créer le fichier SANS FilePath
            var file = new InternFile(
                command.Data.FileName,
                command.Data.FileType,
                command.Data.TraineeId
                // FilePath supprimé ← sera défini après upload
            );

            // 4. Sauvegarder
            await _repository.AddAsync(file);

            // 5. Générer le FilePath automatiquement
            var filePath = $"/uploads/trainees/{file.TraineeId}/{file.Id}_{file.FileName}";
            file.SetFilePath(filePath);
            await _repository.UpdateAsync(file);

            // 6. Retourner DTO
            return new InternFileDto
            {
                Id          = file.Id,
                FileName    = file.FileName,
                FilePath    = file.FilePath,
                FileType    = file.FileType,
                ImportedAt  = file.ImportedAt,
                TraineeId   = file.TraineeId,
                TraineeName = $"{trainee.FirstName} {trainee.LastName}"
            };
        }








    }
}
