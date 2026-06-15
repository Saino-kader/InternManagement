using InterManagement.Domain.Entities;  
using Microsoft.EntityFrameworkCore;

namespace InternManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }


        // ======================================================
        // PROPRIÉTÉS DbSet = REPRÉSENTATION DES TABLES EN SQL
        // Chaque DbSet<T> correspond à UNE table en base de données
        // ======================================================

        // ── Tables ───────────────────────────────
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<Phase> Phases { get; set; }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<WeeklyFollowUp> WeeklyFollowUps { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<InternFile> Files { get; set; }
        public DbSet<Week> Weeks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── TPT : héritage User ───────────────
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Mentor>().ToTable("mentors");
            modelBuilder.Entity<Trainee>().ToTable("trainees");
            modelBuilder.Entity<Phase>().ToTable("phases");

            // ── Autres tables ─────────────────────

            modelBuilder.Entity<Assignment>().ToTable("assignments");
            modelBuilder.Entity<WeeklyFollowUp>().ToTable("weekly_follow_ups");
            modelBuilder.Entity<Feedback>().ToTable("feedbacks");
            modelBuilder.Entity<InternFile>().ToTable("files");
            modelBuilder.Entity<Week>().ToTable("weeks");


            // ── Soft delete global ────────────────
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

            // Relations Assignment
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
                

            // Relations dans OnModelCreating
        
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

            modelBuilder.Entity<WeeklyFollowUp>()
                .HasOne(w => w.Phase)
                .WithMany(p => p.WeeklyFollowUps)
                .HasForeignKey(w => w.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);

                            
            // Relation Feedback
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Trainee)
                .WithMany(t => t.Feedbacks)
                .HasForeignKey(f => f.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);
                


            // Relation
            modelBuilder.Entity<InternFile>()
                .HasOne(f => f.Trainee)
                .WithMany(t => t.Files)
                .HasForeignKey(f => f.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Week>()
                .HasOne(w => w.Phase)
                .WithMany(p => p.Weeks)
                .HasForeignKey(w => w.PhaseId)
                .OnDelete(DeleteBehavior.Restrict);
                

        }
    }
 }    








