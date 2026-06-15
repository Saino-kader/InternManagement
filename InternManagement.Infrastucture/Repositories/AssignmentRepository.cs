using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base 

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .Include(a => a.Trainee)
                .Include(a => a.Mentor)
                .Include(a => a.Phase)
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .Include(a => a.Trainee)
                .Include(a => a.Mentor)
                .Include(a => a.Phase)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Assignment> AddAsync(Assignment entity)
        {
            await _context.Assignments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Assignment entity)
        {
            _context.Assignments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var assignment = await GetByIdAsync(id);
            if (assignment == null) return;

            assignment.IsDeleted = true;
            assignment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales 

        public async Task<Assignment?> GetActiveAssignmentAsync(
            int traineeId, int phaseId)
        {
            return await _context.Assignments
                .Include(a => a.Mentor)
                .Where(a => a.TraineeId == traineeId
                         && a.PhaseId   == phaseId
                         && a.IsActive  == true)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Assignment>> GetByMentorAsync(
            int mentorId)
        {
            return await _context.Assignments
                .Include(a => a.Trainee)
                .Include(a => a.Phase)
                .Where(a => a.MentorId == mentorId
                         && a.IsActive == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetByTraineeAsync(
            int traineeId)
        {
            return await _context.Assignments
                .Include(a => a.Mentor)
                .Include(a => a.Phase)
                .Where(a => a.TraineeId == traineeId)
                .ToListAsync();
        }

        public async Task<bool> AssignmentExistsAsync(
            int traineeId, int mentorId, int phaseId)
        {
            return await _context.Assignments
                .AnyAsync(a => a.TraineeId == traineeId
                            && a.MentorId  == mentorId
                            && a.PhaseId   == phaseId
                            && a.IsActive  == true);
        }
    }
}