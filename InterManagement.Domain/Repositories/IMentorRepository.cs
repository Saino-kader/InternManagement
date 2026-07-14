using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IMentorRepository : IBaseRepository<Mentor>
    {
        Task<IEnumerable<Mentor>> GetByDepartmentAsync(string department);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<Mentor>> GetAllWithTraineeCountAsync();
        Task<int> CountActiveAsync();
    }
}
