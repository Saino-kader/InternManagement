using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IActivityLogRepository
    {
        Task AddAsync(ActivityLog log);
        Task<IEnumerable<ActivityLog>> GetRecentAsync(int count);
    }
}
