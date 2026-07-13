// Infrastructure/Repositories/ImportedFollowUpRepository.cs
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class ImportedFollowUpRepository : IImportedFollowUpRepository
    {
        private readonly AppDbContext _context;

        public ImportedFollowUpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImportedFollowUp>> GetAllAsync()
        {
            return await _context.ImportedFollowUps
                .OrderByDescending(i => i.ImportedAt)
                .ThenBy(i => i.Stagiaire)
                .ToListAsync();
        }

        public async Task<ImportedFollowUp?> GetByIdAsync(int id)
        {
            return await _context.ImportedFollowUps
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<ImportedFollowUp> AddAsync(ImportedFollowUp entity)
        {
            await _context.ImportedFollowUps.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(ImportedFollowUp entity)
        {
            _context.ImportedFollowUps.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item == null) return;

            // Soft delete comme les autres entités
            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ImportedFollowUp>> GetByBatchAsync(string batchId)
        {
            return await _context.ImportedFollowUps
                .Where(i => i.BatchId == batchId)
                .OrderBy(i => i.Stagiaire)
                .ThenBy(i => i.WeekNumber)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ImportedFollowUp> followUps)
        {
            await _context.ImportedFollowUps.AddRangeAsync(followUps);
            await _context.SaveChangesAsync();
        }
    }
}
