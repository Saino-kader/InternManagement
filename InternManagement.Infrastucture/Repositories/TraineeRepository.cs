using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;
using InterManagement.Domain.Repositories;
using InterManagement.Infrastucture.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class TraineeRepository(AppDbContext context) : BaseRepository<Trainee>(context), ITraineeRepository
    {
        // ── Méthodes spéciales 

        public async Task<IEnumerable<Trainee>> GetActiveTraineesAsync()
        {
            return await _context.Trainees
                .Where(t => t.Status == TraineeStatus.InProgress)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trainee>> GetAllWithFiltersAsync(TraineeStatus? status)
        {
            var query = _context.Trainees.AsQueryable();     

            if (status.HasValue)                             
                query = query.Where(t => t.Status == status.Value);

            return await query.ToListAsync();
        }

        /*
        public async Task<Trainee?> GetWithPhasesAsync(int traineeId)
        {
            return await _context.Trainees
                .Include(t => t.Phases)
                .FirstOrDefaultAsync(t => t.Id == traineeId);
        }

        public async Task<Trainee?> GetWithPhasesAndEvaluationsAsync(int traineeId)
        {
            return await _context.Trainees
                .Include(t => t.Phases)
                .FirstOrDefaultAsync(t => t.Id == traineeId);
        }
         */

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Trainees.AnyAsync(t => t.Email == email);
        }

        public Task<Trainee?> GetWithPhasesAsync(int traineeId)

        {
            return _context.Trainees
                .Include(t => t.Email)
                .FirstOrDefaultAsync(t => t.Id == traineeId);
        }

        public Task<Trainee?> GetWithPhasesAndEvaluationsAsync(int traineeId)
        {
            return _context.Trainees
                .Include(t => t.Email)
                .FirstOrDefaultAsync(t => t.Id == traineeId);
        }
    }
}

