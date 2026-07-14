using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IWeekRepository : IBaseRepository<Week>
    {
        /// <summary>Toutes les semaines d'une phase.</summary>
        Task<IEnumerable<Week>> GetByPhaseAsync(int phaseId);

        /// <summary>Empêche de créer deux fois le même numéro de semaine sur une phase.</summary>
        Task<bool> WeekExistsAsync(int phaseId, int weekNumber);
    }
}
