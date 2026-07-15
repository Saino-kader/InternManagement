using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admins
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Admin> AddAsync(Admin entity)
        {
            await _context.Admins.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Admin entity)
        {
            _context.Admins.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var admin = await GetByIdAsync(id);
            if (admin == null) return;

            admin.IsDeleted = true;
            admin.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthode spéciale 

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Admins
                .AnyAsync(a => a.Email == email);
        }
    }
}