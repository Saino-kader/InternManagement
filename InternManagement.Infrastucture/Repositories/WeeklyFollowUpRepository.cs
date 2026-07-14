// Infrastructure/Repositories/WeeklyFollowUpRepository.cs
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
                .Include(w => w.Week)
                .ToListAsync();
        }

        public async Task<WeeklyFollowUp?> GetByIdAsync(int id)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Include(w => w.Week)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WeeklyFollowUp> AddAsync(WeeklyFollowUp entity)
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

        // Suivis pour une phase donnée
        public async Task<IEnumerable<WeeklyFollowUp>> GetByPhaseAsync(
            int phaseId)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Include(w => w.Week)
                .Where(w => w.Week.PhaseId == phaseId)
                .OrderBy(w => w.Week.WeekNumber)
                .ToListAsync();
        }

        // Tous les suivis d'un mentor
        public async Task<IEnumerable<WeeklyFollowUp>> GetByMentorAsync(
            int mentorId)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Week)
                .Where(w => w.MentorId == mentorId)
                .OrderBy(w => w.FollowUpDate)
                .ToListAsync();
        }

        // Vérifie si un suivi existe déjà pour ce Trainee + cette Week
        // Remplace l'ancienne GetByTraineePhaseWeekAsync(traineeId, phaseId, weekNumber)
        // PhaseId et WeekNumber ayant été supprimés de WeeklyFollowUp
        public async Task<WeeklyFollowUp?> GetByTraineeAndWeekAsync(
            int traineeId, int weekId)
        {
            return await _context.WeeklyFollowUps
                .FirstOrDefaultAsync(w =>
                    w.TraineeId == traineeId &&
                    w.WeekId == weekId);
        }

        // Import massif depuis un fichier Excel
        public async Task AddRangeAsync(IEnumerable<WeeklyFollowUp> followUps)
        {
            await _context.WeeklyFollowUps.AddRangeAsync(followUps);
            await _context.SaveChangesAsync();
        }

        // Export — tous les suivis d'un seul stagiaire
        public async Task<IEnumerable<WeeklyFollowUp>> GetByTraineeForExportAsync(
            int traineeId)
        {
            return await _context.WeeklyFollowUps
                .Include(w => w.Trainee)
                .Include(w => w.Mentor)
                .Include(w => w.Week)
                .Where(w => w.TraineeId == traineeId)
                .OrderBy(w => w.FollowUpDate)
                .ToListAsync();
        }

        // Recherche par nom complet pour l'import Excel
        public async Task<int?> FindTraineeIdByFullNameAsync(string fullName)
        {
            var trainee = await _context.Trainees
                .FirstOrDefaultAsync(t =>
                    (t.FirstName + " " + t.LastName).ToLower()
                    == fullName.Trim().ToLower());
            return trainee?.Id;
        }

        public async Task<int?> FindMentorIdByFullNameAsync(string fullName)
        {
            var mentor = await _context.Mentors
                .FirstOrDefaultAsync(m =>
                    (m.FirstName + " " + m.LastName).ToLower()
                    == fullName.Trim().ToLower());
            return mentor?.Id;
        }
    }
}
