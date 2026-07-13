using InterManagement.Domain.Repositories;

namespace InterManagement.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // ── Repositories 
        ITraineeRepository Trainees { get; }
        IMentorRepository Mentors { get; }
        
        IAdminRepository Admins { get; }
        IPhaseRepository Phases { get; }
        IAssignmentRepository Assignments { get; }
        IWeeklyFollowUpRepository WeeklyFollowUps { get; }
        IFeedbackRepository Feedbacks { get; }
        // ── Transaction 
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
