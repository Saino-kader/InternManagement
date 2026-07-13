// Domain/Repositories/IWeeklyFollowUpRepository.cs
using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IWeeklyFollowUpRepository
        : IBaseRepository<WeeklyFollowUp>
    {
        Task<IEnumerable<WeeklyFollowUp>> GetByPhaseAsync(int phaseId);
        Task<IEnumerable<WeeklyFollowUp>> GetByMentorAsync(int mentorId);

        Task<WeeklyFollowUp?> GetByTraineeAndWeekAsync(int traineeId, int weekId);

        // Import massif Excel
        Task AddRangeAsync(IEnumerable<WeeklyFollowUp> followUps);

        // Export — tous les suivis d'un seul stagiaire
        Task<IEnumerable<WeeklyFollowUp>> GetByTraineeForExportAsync(int traineeId);

        // Recherche par nom complet pour l'import Excel
        Task<int?> FindTraineeIdByFullNameAsync(string fullName);
        Task<int?> FindMentorIdByFullNameAsync(string fullName);
    }
}
