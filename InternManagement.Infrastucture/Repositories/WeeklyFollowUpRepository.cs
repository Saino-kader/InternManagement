using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class WeeklyFollowUpRepository : IWeeklyFollowUpRepository
    {
        private readonly AppDbContext _context;

        public WeeklyFollowUpRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<WeeklyFollowUp>> GetAllAsync()
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Include(w => w.Phase)
                .ToListAsync();
        }

        public async Task<WeeklyFollowUp?> GetByIdAsync(int id)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Include(w => w.Phase)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WeeklyFollowUp> AddAsync(
            WeeklyFollowUp entity)
        {
            await _context.WeeklyFollowUps.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(WeeklyFollowUp entity)
        {
            _context.WeeklyFollowUps.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var followUp = await GetByIdAsync(id);
            if (followUp == null) return;

            followUp.IsDeleted = true;
            followUp.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales 

        public async Task<IEnumerable<WeeklyFollowUp>> GetByPhaseAsync(
            int phaseId)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Where(w => w.PhaseId == phaseId)
                .OrderBy(w => w.WeekNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<WeeklyFollowUp>> GetByMentorAsync(
            int mentorId)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Phase)
                .Where(w => w.MentorId == mentorId)
                .OrderBy(w => w.FollowUpDate)
                .ToListAsync();
        }

        public async Task<WeeklyFollowUp?> GetByTraineePhaseWeekAsync(
            int traineeId, int phaseId, int weekNumber)
        {
            return await _context.WeeklyFollowUps
                .FirstOrDefaultAsync(w =>
                    w.TraineeId  == traineeId &&
                    w.PhaseId    == phaseId   &&
                    w.WeekNumber == weekNumber);
        }

        public async Task AddRangeAsync(
            IEnumerable<WeeklyFollowUp> followUps)
        {
            await _context.WeeklyFollowUps.AddRangeAsync(followUps);
            await _context.SaveChangesAsync();
        }
    }
}
