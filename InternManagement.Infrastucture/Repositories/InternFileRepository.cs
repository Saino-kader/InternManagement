using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class InternFileRepository : IInternFileRepository
    {
        private readonly AppDbContext _context;

        public InternFileRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<InternFile>> GetAllAsync()
        {
            return await _context.Files
                .Include(f => f.Trainee)
                .ToListAsync();
        }

        public async Task<InternFile?> GetByIdAsync(int id)
        {
            return await _context.Files
                .Include(f => f.Trainee)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<InternFile> AddAsync(InternFile entity)
        {
            await _context.Files.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(InternFile entity)
        {
            _context.Files.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var file = await GetByIdAsync(id);
            if (file == null) return;

            file.IsDeleted = true;
            file.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales 

        public async Task<IEnumerable<InternFile>> GetByTraineeAsync(
            int traineeId)
        {
            return await _context.Files
                .Where(f => f.TraineeId == traineeId)
                .OrderByDescending(f => f.ImportedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InternFile>> GetByTypeAsync(
            int traineeId, string fileType)
        {
            return await _context.Files
                .Where(f => f.TraineeId == traineeId
                         && f.FileType  == fileType.ToUpper())
                .OrderByDescending(f => f.ImportedAt)
                .ToListAsync();
        }

        public async Task<bool> FileExistsAsync(
            int traineeId, string fileName)
        {
            return await _context.Files
                .AnyAsync(f => f.TraineeId == traineeId
                            && f.FileName  == fileName
                            && !f.IsDeleted);
        }
    }
}
