// Domain/Repositories/IFeedbackRepository.cs
using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IFeedbackRepository : IBaseRepository<Feedback>
    {
        // Stagiaire → ses feedbacks reçus
        Task<IEnumerable<Feedback>> GetByTraineeAsync(int traineeId);

        // Mentor → ses feedbacks envoyés
        // NOUVEAU : évite le filtre nullable côté Controller
        Task<IEnumerable<Feedback>> GetByMentorAsync(int mentorId);

        // Admin → feedbacks récents d'un stagiaire
        Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(
            int traineeId, int count);
    }
}