using InterManagement.Application.Common;
using InterManagement.Application.Features.Admins.Commands.CreateAdmin;
using InterManagement.Application.Features.Admins.Commands.DeleteAdmin;
using InterManagement.Application.Features.Admins.Commands.UpdateAdmin;
using InterManagement.Application.Features.Admins.Queries.GetAdminById;
using InterManagement.Application.Features.Admins.Queries.GetAdmins;
using InterManagement.Application.Features.Assignments.Commands.CreateAssignment;
using InterManagement.Application.Features.Assignments.Commands.DeactivateAssignment;
using InterManagement.Application.Features.Assignments.Commands.DeleteAssignment;
using InterManagement.Application.Features.Assignments.Queries.GetAssignmentById;
using InterManagement.Application.Features.Assignments.Queries.GetAssignments;
using InterManagement.Application.Features.Assignments.Queries.GetMentorAssignments;
using InterManagement.Application.Features.Dashboard.Queries.GetDashboardStats;
using InterManagement.Application.Features.Dashboard.Queries.GetRecentActivity;
using InterManagement.Application.Features.Feedbacks.Commands.CreateFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.DeleteFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.UpdateFeedback;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbackById;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbacks;
using InterManagement.Application.Features.ImportedFollowUps.Commands.DeleteImportedFollowUp;
using InterManagement.Application.Features.ImportedFollowUps.Commands.ImportExcel;
using InterManagement.Application.Features.ImportedFollowUps.Commands.UpdateImportedFollowUp;
using InterManagement.Application.Features.ImportedFollowUps.Queries.GetImportedFollowUps;
using InterManagement.Application.Features.Mentors.Commands.CreateMentor;
using InterManagement.Application.Features.Mentors.Commands.DeleteMentor;
using InterManagement.Application.Features.Mentors.Commands.UpdateMentor;
using InterManagement.Application.Features.Mentors.Queries.GetMentorById;
using InterManagement.Application.Features.Mentors.Queries.GetMentors;
using InterManagement.Application.Features.Phases.Commands.CreatePhase;
using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
using InterManagement.Application.Features.Phases.Commands.DeletePhase;
using InterManagement.Application.Features.Phases.Commands.UpdatePhase;
using InterManagement.Application.Features.Phases.Queries.GetPhaseById;
using InterManagement.Application.Features.Phases.Queries.GetPhases;
using InterManagement.Application.Features.Trainees.Commands.CreateTrainee;
using InterManagement.Application.Features.Trainees.Commands.DeleteTrainee;
using InterManagement.Application.Features.Trainees.Commands.UpdateTrainee;
using InterManagement.Application.Features.Trainees.Queries.GetTraineeById;
using InterManagement.Application.Features.Trainees.Queries.GetTrainees;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.CreateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.DeleteWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.SuspendedCommand;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ValidatedFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Export;
using InterManagement.Application.Features.WeeklyFollowUps.Import;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.ExportWeeklyFollowUpsByTrainee;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUps;
using InterManagement.Application.Features.Weeks.Commands.CreateWeek;
using InterManagement.Application.Features.Weeks.Commands.DeleteWeek;
using InterManagement.Application.Features.Weeks.Commands.UpdateWeek;
using InterManagement.Application.Features.Weeks.Queries.GetWeekById;
using InterManagement.Application.Features.Weeks.Queries.GetWeeks;
using Microsoft.Extensions.DependencyInjection;

namespace InterManagement.Application
{
    /// <summary>
    /// Enregistre tous les handlers CQRS de la couche Application dans le
    /// conteneur d'injection de dépendances.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // ── Stagiaires ─────────────────────────────
            services.AddScoped<CreateTraineeHandler>();
            services.AddScoped<UpdateTraineeHandler>();
            services.AddScoped<DeleteTraineeHandler>();
            services.AddScoped<GetTraineesHandler>();
            services.AddScoped<GetTraineeByIdHandler>();

            // ── Mentors ────────────────────────────────
            services.AddScoped<CreateMentorHandler>();
            services.AddScoped<UpdateMentorHandler>();
            services.AddScoped<DeleteMentorHandler>();
            services.AddScoped<GetMentorsHandler>();
            services.AddScoped<GetMentorByIdHandler>();

            // ── Admins ─────────────────────────────────
            services.AddScoped<CreateAdminHandler>();
            services.AddScoped<UpdateAdminHandler>();
            services.AddScoped<DeleteAdminHandler>();
            services.AddScoped<GetAdminsHandler>();
            services.AddScoped<GetAdminByIdHandler>();

            // ── Phases ─────────────────────────────────
            services.AddScoped<CreatePhaseHandler>();
            services.AddScoped<UpdatePhaseHandler>();
            services.AddScoped<DeletePhaseHandler>();
            services.AddScoped<GetPhasesHandler>();
            services.AddScoped<GetPhaseByIdHandler>();
            services.AddScoped<CreatePhaseForMultipleTraineesHandler>();

            // ── Affectations mentor/stagiaire ─────────
            services.AddScoped<CreateAssignmentHandler>();
            services.AddScoped<DeactivateAssignmentHandler>();
            services.AddScoped<DeleteAssignmentHandler>();
            services.AddScoped<GetAssignmentsHandler>();
            services.AddScoped<GetAssignmentByIdHandler>();
            services.AddScoped<GetMentorAssignmentsHandler>();

            // ── Semaines ───────────────────────────────
            services.AddScoped<CreateWeekHandler>();
            services.AddScoped<UpdateWeekHandler>();
            services.AddScoped<DeleteWeekHandler>();
            services.AddScoped<GetWeeksHandler>();
            services.AddScoped<GetWeekByIdHandler>();

            // ── Suivis hebdomadaires ───────────────────
            services.AddScoped<CreateWeeklyFollowUpHandler>();
            services.AddScoped<ValidatedFollowUpHandler>();
            services.AddScoped<SuspendedHandler>();
            services.AddScoped<DeleteWeeklyFollowUpHandler>();
            services.AddScoped<GetWeeklyFollowUpsHandler>();
            services.AddScoped<GetWeeklyFollowUpByIdHandler>();
            services.AddScoped<UpdateWeeklyFollowUpHandler>();

            // ── Import / export Excel des suivis hebdomadaires ──
            services.AddScoped<IWeeklyFollowUpFileParser, WeeklyFollowUpFileParser>();
            services.AddScoped<IWeeklyFollowUpExportService, WeeklyFollowUpExportService>();
            services.AddScoped<ImportWeeklyFollowUpsHandler>();
            services.AddScoped<ExportWeeklyFollowUpsByTraineeHandler>();

            // ── Suivis importés depuis Excel (données brutes) ───
            services.AddScoped<GetImportedFollowUpsHandler>();
            services.AddScoped<ImportExcelHandler>();
            services.AddScoped<UpdateImportedFollowUpHandler>();
            services.AddScoped<DeleteImportedFollowUpHandler>();

            // ── Feedbacks / messages ───────────────────
            services.AddScoped<CreateFeedbackHandler>();
            services.AddScoped<UpdateFeedbackHandler>();
            services.AddScoped<DeleteFeedbackHandler>();
            services.AddScoped<GetFeedbacksHandler>();
            services.AddScoped<GetFeedbackByIdHandler>();

            // ── Dashboard ──────────────────────────────
            services.AddScoped<GetDashboardStatsHandler>();
            services.AddScoped<GetRecentActivityHandler>();

            // ── Journal d'activité ─────────────────────
            services.AddScoped<IActivityLogger, ActivityLogger>();

            return services;
        }
    }
}
