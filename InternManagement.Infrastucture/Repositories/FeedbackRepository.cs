// Infrastructure/Repositories/FeedbackRepository.cs
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

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .Include(f => f.Mentor)
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        }

        public async Task<Feedback?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .Include(f => f.Mentor)
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

        // Feedbacks reçus par un stagiaire
        public async Task<IEnumerable<Feedback>> GetByTraineeAsync(int traineeId)
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .Include(f => f.Mentor)
                .Where(f => f.TraineeId == traineeId)
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        }

        // Feedbacks envoyés par un mentor (exclut les messages reçus d'un
        // stagiaire, qui ont aussi MentorId renseigné mais un TraineeId)
        public async Task<IEnumerable<Feedback>> GetByMentorAsync(int mentorId)
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .Include(f => f.Mentor)
                .Where(f => f.MentorId == mentorId && f.TraineeId == null)
                .OrderByDescending(f => f.SentAt)
                .ToListAsync();
        }

        // Feedbacks récents d'un stagiaire
        public async Task<IEnumerable<Feedback>> GetRecentFeedbacksAsync(
            int traineeId, int count)
        {
            return await _context.Feedbacks
                .Include(f => f.Trainee)
                .Include(f => f.Mentor)
                .Where(f => f.TraineeId == traineeId)
                .OrderByDescending(f => f.SentAt)
                .Take(count)
                .ToListAsync();
        }
    }
}