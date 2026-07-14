using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class WeekRepository : IWeekRepository
    {
        private readonly AppDbContext _context;

        public WeekRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<Week>> GetAllAsync()
        {
            return await _context.Weeks
                .Include(w => w.Phase)
                .ToListAsync();
        }

        public async Task<Week?> GetByIdAsync(int id)
        {
            return await _context.Weeks
                .Include(w => w.Phase)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Week> AddAsync(Week entity)
        {
            await _context.Weeks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Week entity)
        {
            _context.Weeks.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var week = await GetByIdAsync(id);
            if (week == null) return;

            week.IsDeleted = true;
            week.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales 

        public async Task<IEnumerable<Week>> GetByPhaseAsync(
            int phaseId)
        {
            return await _context.Weeks
                .Include(w => w.Phase)
                .Where(w => w.PhaseId == phaseId)
                .OrderBy(w => w.WeekNumber)
                .ToListAsync();
        }

        public async Task<bool> WeekExistsAsync(
            int phaseId, int weekNumber)
        {
            return await _context.Weeks
                .AnyAsync(w => w.PhaseId    == phaseId
                            && w.WeekNumber == weekNumber
                            && !w.IsDeleted);
        }
    }
}
