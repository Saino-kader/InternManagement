using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class MentorRepository : IMentorRepository
    {
        private readonly AppDbContext _context;

        public MentorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mentor>> GetAllAsync()
        {
            return await _context.Mentors.ToListAsync();
        }

        public async Task<Mentor?> GetByIdAsync(int id)
        {
            return await _context.Mentors
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mentor> AddAsync(Mentor entity)
        {
            await _context.Mentors.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Mentor entity)
        {
            _context.Mentors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mentor = await GetByIdAsync(id);
            if (mentor == null) return;

            mentor.IsDeleted = true;
            mentor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Mentor>> GetByDepartmentAsync(string department)
        {
            return await _context.Mentors
                .Include(m => m.Assignments)
                .Where(m => m.Department == department)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Mentors
                .AnyAsync(m => m.Email == email);
        }

        public async Task<IEnumerable<Mentor>> GetAllWithTraineeCountAsync()
        {
            return await _context.Mentors
                .Include(m => m.Assignments)
                .ToListAsync();
        }

        // ← C'EST CETTE MÉTHODE QUI MANQUE PHYSIQUEMENT
        public async Task<int> CountActiveAsync()
        {
            return await _context.Mentors
                .CountAsync(m => m.IsActive);
        }
    }
}




































/*
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class MentorRepository : IMentorRepository
    {
        private readonly AppDbContext _context;

        public MentorRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<Mentor>> GetAllAsync()
        {
            return await _context.Mentors
                .ToListAsync();
        }

        public async Task<Mentor?> GetByIdAsync(int id)
        {
            return await _context.Mentors
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mentor> AddAsync(Mentor entity)
        {
            await _context.Mentors.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Mentor entity)
        {
            _context.Mentors.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var mentor = await GetByIdAsync(id);
            if (mentor == null) return;

            mentor.IsDeleted = true;
            mentor.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales ────────────────

        public async Task<IEnumerable<Mentor>> GetByDepartmentAsync(
            string department)
        {
            return await _context.Mentors
                .Where(m => m.Department == department)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Mentors
                .AnyAsync(m => m.Email == email);
        }


        public async Task<IEnumerable<Mentor>> GetAllWithTraineeCountAsync()
        {
            return await _context.Mentors
                //.Include(m => m.Assignments)
                .ToListAsync();
        }

    }
}

*/
