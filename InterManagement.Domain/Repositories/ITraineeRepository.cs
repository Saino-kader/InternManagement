using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Domain.Repositories
{
    public interface ITraineeRepository : IBaseRepository<Trainee>
    {
        Task<IEnumerable<Trainee>> GetActiveTraineesAsync();
        Task<IEnumerable<Trainee>> GetAllWithFiltersAsync(TraineeStatus? status);
        Task<Trainee?> GetWithPhasesAsync(int traineeId);
        Task<Trainee?> GetWithPhasesAndEvaluationsAsync(int traineeId);
        Task<bool> EmailExistsAsync(string email);
        Task<int> CountByStatusAsync(TraineeStatus status);
    }
}
