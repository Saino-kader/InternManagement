// Application/Features/ImportedFollowUps/Commands/UpdateImportedFollowUp/
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.ImportedFollowUps.Commands.UpdateImportedFollowUp
{
    public class UpdateImportedFollowUpCommand
    {
        public int Id { get; set; }
        public UpdateImportedFollowUpDto Data { get; set; } = null!;
    }

    public class UpdateImportedFollowUpHandler
    {
        private readonly IImportedFollowUpRepository _repository;

        public UpdateImportedFollowUpHandler(IImportedFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task<ImportedFollowUpDto> Handle(UpdateImportedFollowUpCommand command)
        {
            var item = await _repository.GetByIdAsync(command.Id);
            if (item == null)
                throw new DomainException($"Suivi importé #{command.Id} introuvable");

            item.Update(
                command.Data.Cours,
                command.Data.Appreciation,
                command.Data.Commentaire,
                command.Data.Statut
            );

            await _repository.UpdateAsync(item);

            return new ImportedFollowUpDto
            {
                Id           = item.Id,
                Stagiaire    = item.Stagiaire,
                Mentor       = item.Mentor,
                Date         = item.Date,
                WeekNumber   = item.WeekNumber,
                Cours        = item.Cours,
                Appreciation = item.Appreciation,
                Commentaire  = item.Commentaire,
                Statut       = item.Statut,
                ImportedAt   = item.ImportedAt,
                BatchId      = item.BatchId
            };
        }
    }
}
