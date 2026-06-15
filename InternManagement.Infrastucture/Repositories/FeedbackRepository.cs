using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly AppDbContext _context;

        public FeedbackRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.Include(f => f.Trainee).ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feedback> AddAsync(Feedback entity)
        {
            await _context.Feedbacks.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Feedback entity)
        {
            _context.Feedbacks.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var feedback = await GetByIdAsync(id);
            if (feedback == null) return;

            feedback.IsDeleted = true;
            feedback.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales 

        public async Task<IEnumerable<Feedback>> GetByTraineeAsync(
            int traineeId)
        {
            return await _context.Feedbacks
                .Where(f => f.TraineeId == traineeId)
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(
            int traineeId, int count)
        {
            return await _context.Feedbacks
                .Where(f => f.TraineeId == traineeId)
                .OrderByDescending(f => f.SentAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
