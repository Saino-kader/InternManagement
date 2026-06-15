using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;


namespace InterManagement.Domain.Repositories
{
    public interface ITraineeRepository : IBaseRepository<Trainee>
    {
        // Admin → liste filtrée par statut
        Task<IEnumerable<Trainee>> GetActiveTraineesAsync();

        // Admin → tous avec filtre optionnel
        Task<IEnumerable<Trainee>> GetAllWithFiltersAsync(TraineeStatus? status);

        // Mentor + Stagiaire → détail avec phases
         Task<Trainee?> GetWithPhasesAsync(int traineeId);

        // Mentor → détail avec phases ET évaluations
         Task<Trainee?> GetWithPhasesAndEvaluationsAsync(int traineeId);
        
        Task<bool> EmailExistsAsync(string email);  
    }
}

