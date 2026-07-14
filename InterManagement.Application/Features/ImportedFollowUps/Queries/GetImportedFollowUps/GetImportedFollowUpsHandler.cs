// Application/Features/ImportedFollowUps/Queries/GetImportedFollowUps/GetImportedFollowUpsHandler.cs
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.ImportedFollowUps.Queries.GetImportedFollowUps
{
    public class GetImportedFollowUpsQuery { }

    public class GetImportedFollowUpsHandler
    {
        private readonly IImportedFollowUpRepository _repository;

        public GetImportedFollowUpsHandler(IImportedFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ImportedFollowUpDto>> Handle(
            GetImportedFollowUpsQuery query)
        {
            var items = await _repository.GetAllAsync();

            return items.Select(i => new ImportedFollowUpDto
            {
                Id           = i.Id,
                Stagiaire    = i.Stagiaire,
                Mentor       = i.Mentor,
                Date         = i.Date,
                WeekNumber   = i.WeekNumber,
                Cours        = i.Cours,
                Appreciation = i.Appreciation,
                Commentaire  = i.Commentaire,
                Statut       = i.Statut,
                ImportedAt   = i.ImportedAt,
                BatchId      = i.BatchId
            });
        }
    }
}
