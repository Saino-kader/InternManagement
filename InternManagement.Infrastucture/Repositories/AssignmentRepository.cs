// Infrastructure/Repositories/AssignmentRepository.cs
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

        // ── Récupère tous les assignments actifs d'un mentor ──────────
        // Charge Trainee + Phase + Weeks pour que le Handler puisse
        // construire AssignmentDto sans appels supplémentaires
        public async Task<IEnumerable<Assignment>> GetByMentorAsync(int mentorId)
        {
            return await _context.Assignments
                .Include(a => a.Trainee)
                .Include(a => a.Mentor)
                .Include(a => a.Phase)
                    .ThenInclude(p => p.Weeks)
                        .ThenInclude(w => w.WeeklyFollowUps)
                .Where(a => a.MentorId == mentorId && a.IsActive)
                .OrderBy(a => a.Trainee.LastName)
                .ToListAsync();
        }


        /*


        public async Task<IEnumerable<Assignment>> GetByMentorAsync(int mentorId)
        {
            return await _context.Assignments
                .Include(a => a.Trainee)                                     // ← Stagiaire
                .Include(a => a.Phase)                                       // ← Phase
                    .ThenInclude(p => p.Weeks)                               // ← Semaines
                        .ThenInclude(w => w.WeeklyFollowUps)                 // ← WeeklyFollowUp (pour Statut)
                .Where(a => a.MentorId == mentorId
                        && a.IsActive == true)
                .ToListAsync();
        }
*/
        // ── Récupère tous les assignments d'un trainee ────────────────
        public async Task<IEnumerable<Assignment>> GetByTraineeAsync(int traineeId)
        {
            return await _context.Assignments
                .Include(a => a.Mentor)
                .Include(a => a.Phase)
                .Where(a => a.TraineeId == traineeId)
                .OrderByDescending(a => a.AssignmentDate)
                .ToListAsync();
        }

        // ── Vérifie si un assignment existe déjà ─────────────────────
        public async Task<bool> AssignmentExistsAsync(
            int traineeId, int mentorId, int phaseId)
        {
            return await _context.Assignments
                .AnyAsync(a =>
                    a.TraineeId == traineeId &&
                    a.MentorId == mentorId &&
                    a.PhaseId == phaseId &&
                    a.IsActive);
        }

        // ── Récupère l'assignment actif d'un trainee sur une phase ────
        public async Task<Assignment?> GetActiveAssignmentAsync(
            int traineeId, int phaseId)
        {
            return await _context.Assignments
                .Include(a => a.Mentor)
                .FirstOrDefaultAsync(a =>
                    a.TraineeId == traineeId &&
                    a.PhaseId == phaseId &&
                    a.IsActive);
        }
    }
}