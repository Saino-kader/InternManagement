using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IWeekRepository : IBaseRepository<Week>
    {
        // Phase → ses semaines
        Task<IEnumerable<Week>> GetByPhaseAsync(int phaseId);

        // Vérifier numéro semaine pas déjà existant
        Task<bool> WeekExistsAsync(int phaseId, int weekNumber);

        // Supprimer par phase et numéro
        Task DeleteByPhaseAndNumberAsync(
            int phaseId, int weekNumber);
    }
}