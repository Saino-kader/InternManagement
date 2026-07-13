/*
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InterManagement.Infrastucture.Repositories;
using InterManagement.Shared.Enums;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Repositories
{
    public class PhaseRepository : BaseRepository<Phase>, IPhaseRepository
    {
        public PhaseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Phase>> GetByTraineeAsync(int traineeId)
        {
            return await _context.Phases
                .Include(p => p.Weeks)
                .Where(p => p.TraineeId == traineeId)
                .OrderBy(p => p.PhaseNumber)
                .ToListAsync();
        }

        public async Task<Phase?> GetCurrentPhaseAsync(int traineeId)
        {
            return await _context.Phases
                .Include(p => p.Weeks)
                .Where(p => p.TraineeId == traineeId && p.Status == PhaseStatus.InProgress)
                .FirstOrDefaultAsync();
        }

        public async Task<Phase?> GetWithFollowUpsAsync(int phaseId)
        {
            return await _context.Phases
                .Include(p => p.Weeks)
                    .ThenInclude(w => w.WeeklyFollowUps)
                .FirstOrDefaultAsync(p => p.Id == phaseId);
        }
    }
}

*/






// Infrastructure/Repositories/PhaseRepository.cs
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using InternManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using InterManagement.Shared.Enums;

namespace InternManagement.Infrastructure.Repositories
{
    public class PhaseRepository : IPhaseRepository
    {
        private readonly AppDbContext _context;

        public PhaseRepository(AppDbContext context)
        {
            _context = context;
        }

        // ── CRUD de base ─────────────────────────────────────────────

        public async Task<IEnumerable<Phase>> GetAllAsync()
        {
            return await _context.Phases
                .Include(p => p.Trainee)           // ← AJOUTÉ pour afficher le nom du stagiaire
                .Include(p => p.Weeks)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Trainee)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Mentor)
                .OrderBy(p => p.PhaseNumber)
                .ThenBy(p => p.Trainee.LastName)   // ← tri cohérent
                .ToListAsync();
        }

        public async Task<Phase?> GetByIdAsync(int id)
        {
            return await _context.Phases
                .Include(p => p.Trainee)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Phase>> GetByTraineeAsync(int traineeId)
        {
            return await _context.Phases
                .Include(p => p.Trainee)
                .Include(p => p.Weeks)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Trainee)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Mentor)
                .Where(p => p.TraineeId == traineeId)
                .OrderBy(p => p.PhaseNumber)
                .ToListAsync();
        }

        public async Task<Phase> AddAsync(Phase entity)
        {
            await _context.Phases.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Phase entity)
        {
            _context.Phases.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var phase = await GetByIdAsync(id);
            if (phase == null) return;

            phase.IsDeleted = true;
            phase.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // ── Méthodes spéciales ────────────────────────────────────────

        public async Task<Phase?> GetCurrentPhaseAsync(int traineeId)
        {
            return await _context.Phases
                .Where(p => p.TraineeId == traineeId
                         && p.Status   == PhaseStatus.InProgress)
                .FirstOrDefaultAsync();
        }

        public async Task<Phase?> GetWithFollowUpsAsync(int phaseId)
        {
            return await _context.Phases
                .Include(p => p.Trainee)
                .Include(p => p.Weeks)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Trainee)
                .Include(p => p.Assignments)
                    .ThenInclude(a => a.Mentor)
                .FirstOrDefaultAsync(p => p.Id == phaseId);
        }

        public async Task<Phase?> GetWithWeeksAsync(int phaseId)
        {
            return await _context.Phases
                .Include(p => p.Weeks)
                .FirstOrDefaultAsync(p => p.Id == phaseId);
        }
    }
}