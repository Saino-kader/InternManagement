using InterManagement.Domain.Entities;

namespace InterManagement.Domain.Repositories
{
    public interface IAdminRepository : IBaseRepository<Admin>
    {
        // vérifier email existant
        Task<bool> EmailExistsAsync(string email);
    }
}

