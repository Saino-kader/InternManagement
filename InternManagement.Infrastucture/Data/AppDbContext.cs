using InterManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // ══════════════════════════════════════════════════════
        // PROPRIÉTÉS DbSet = REPRÉSENTATION DES TABLES EN SQL
        // Chaque DbSet<T> correspond à UNE table en base de données
        // ══════════════════════════════════════════════════════

        public DbSet<User>            AppUsers      { get; set; }
        public DbSet<Admin>           Admins        { get; set; }
        public DbSet<Mentor>          Mentors       { get; set; }
        public DbSet<Trainee>         Trainees      { get; set; }
        public DbSet<Phase>           Phases        { get; set; }
        public DbSet<Assignment>      Assignments   { get; set; }
        public DbSet<WeeklyFollowUp>  WeeklyFollowUps { get; set; }
        public DbSet<Feedback>        Feedbacks     { get; set; }
        public DbSet<Week>            Weeks         { get; set; }
        public DbSet<ActivityLog>     ActivityLogs  { get; set; }
        public DbSet<ImportedFollowUp> ImportedFollowUps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── TPT : héritage User ───────────────────────────
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Mentor>().ToTable("mentors");
            modelBuilder.Entity<Trainee>().ToTable("trainees");
            modelBuilder.Entity<Phase>().ToTable("phases");

            // ── Autres tables ─────────────────────────────────
            modelBuilder.Entity<Assignment>().ToTable("assignments");
            modelBuilder.Entity<WeeklyFollowUp>().ToTable("weekly_follow_ups");
            modelBuilder.Entity<Feedback>().ToTable("feedbacks");
            modelBuilder.Entity<Week>().ToTable("weeks");
            modelBuilder.Entity<ActivityLog>().ToTable("activity_logs");
            modelBuilder.Entity<ImportedFollowUp>().ToTable("imported_followups");

            // ── Soft delete global ────────────────────────────
            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Phase>()
                .HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<WeeklyFollowUp>()
                .HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Assignment>()
                .HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<Week>()
                .HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Feedback>()
                .HasQueryFilter(f => !f.IsDeleted);
            modelBuilder.Entity<ImportedFollowUp>()
                .HasQueryFilter(i => !i.IsDeleted);

            // ── Relations Assignment ──────────────────────────
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Trainee)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Mentor)
                .WithMany(m => m.Assignments)
                .HasForeignKey(a => a.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Phase)
                .WithMany(p => p.Assignments)
                .HasForeignKey(a => a.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relations WeeklyFollowUp ──────────────────────
            modelBuilder.Entity<WeeklyFollowUp>()
                .HasOne(w => w.Trainee)
                .WithMany(t => t.WeeklyFollowUps)
                .HasForeignKey(w => w.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WeeklyFollowUp>()
                .HasOne(w => w.Mentor)
                .WithMany(m => m.WeeklyFollowUps)
                .HasForeignKey(w => w.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relations Feedback ────────────────────────────
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Trainee)
                .WithMany(t => t.Feedbacks)
                .HasForeignKey(f => f.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Mentor)
                .WithMany(m => m.Feedbacks)
                .HasForeignKey(f => f.MentorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── Relations Week ────────────────────────────────
            modelBuilder.Entity<Week>()
                .HasOne(w => w.Phase)
                .WithMany(p => p.Weeks)
                .HasForeignKey(w => w.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
