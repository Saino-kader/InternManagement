// Domain/Repositories/IImportedFollowUpRepository.cs
using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IImportedFollowUpRepository
        : IBaseRepository<ImportedFollowUp>
    {
        // Récupère tous les imports d'un batch spécifique
        Task<IEnumerable<ImportedFollowUp>> GetByBatchAsync(string batchId);

        // Import massif depuis Excel
        Task AddRangeAsync(IEnumerable<ImportedFollowUp> followUps);
    }
}
