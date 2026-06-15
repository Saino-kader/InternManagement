using InterManagement.Application.Features.Trainees.Commands.CreateTrainee;
using InterManagement.Application.Features.Trainees.Commands.UpdateTrainee;
using InterManagement.Application.Features.Trainees.Commands.DeleteTrainee;
using InterManagement.Application.Features.Trainees.Queries.GetTrainees;
using InterManagement.Application.Features.Trainees.Queries.GetTraineeById;
using Microsoft.Extensions.DependencyInjection;
using InterManagement.Application.Features.Mentors.Commands.CreateMentor;
using InterManagement.Application.Features.Mentors.Commands.UpdateMentor;
using InterManagement.Application.Features.Mentors.Commands.DeleteMentor;
using InterManagement.Application.Features.Mentors.Queries.GetMentors;
using InterManagement.Application.Features.Mentors.Queries.GetMentorById;
using InterManagement.Application.Features.Admins.Commands.CreateAdmin;
using InterManagement.Application.Features.Admins.Commands.DeleteAdmin;
using InterManagement.Application.Features.Admins.Commands.UpdateAdmin;
using InterManagement.Application.Features.Admins.Queries.GetAdmins;
using InterManagement.Application.Features.Admins.Queries.GetAdminById;
using InterManagement.Application.Features.Phases.Commands.CreatePhase;
using InterManagement.Application.Features.Phases.Commands.DeletePhase;
using InterManagement.Application.Features.Phases.Commands.UpdatePhase;
using InterManagement.Application.Features.Phases.Queries.GetPhases;
using InterManagement.Application.Features.Phases.Queries.GetPhaseById;
using InterManagement.Application.Features.Assignments.Commands.CreateAssignment;
using InterManagement.Application.Features.Assignments.Commands.DeactivateAssignment;
using InterManagement.Application.Features.Assignments.Queries.GetAssignments;
using InterManagement.Application.Features.Assignments.Commands.DeleteAssignment;
using InterManagement.Application.Features.Assignments.Queries.GetAssignmentById;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.CreateWeeklyFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById;
using InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUps;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.DeleteWeeklyFollowUp;
using InterManagement.Application.Features.Feedbacks.Commands.CreateFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.UpdateFeedback;
using InterManagement.Application.Features.Feedbacks.Commands.DeleteFeedback;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbacks;
using InterManagement.Application.Features.Feedbacks.Queries.GetFeedbackById;
using InterManagement.Application.Features.InternFiles.Queries.GetInternFileById;
using InterManagement.Application.Features.InternFiles.Queries.GetInternFiles;
using InterManagement.Application.Features.InternFiles.Commands.DeleteInternFile;
using InterManagement.Application.Features.InternFiles.Commands.CreateInternFile;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.CompleteFollowUp;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.MarkMissed;
using InterManagement.Application.Features.Weeks.Commands.CreateWeek;
using InterManagement.Application.Features.Weeks.Commands.UpdateWeek;
using InterManagement.Application.Features.Weeks.Commands.DeleteWeek;
using InterManagement.Application.Features.Weeks.Queries.GetWeeks;
using InterManagement.Application.Features.Weeks.Queries.GetWeekById;
using InterManagement.Application.Features.InternFiles.Commands.UpdateInternFile;
using InterManagement.Application.Features.InternFiles.Commands.SetFilePath;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp;

namespace InterManagement.Application
{
    // ======================================================
    // CLASSE STATIQUE D'INJECTION DE DÉPENDANCES POUR LA COUCHE APPLICATION
    // ======================================================
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services) 
        {

            services.AddScoped<CreateTraineeHandler>();
            services.AddScoped<UpdateTraineeHandler>();
            services.AddScoped<DeleteTraineeHandler>();
            services.AddScoped<GetTraineesHandler>();
            services.AddScoped<GetTraineeByIdHandler>();

                // Pour Mentors
            services.AddScoped<CreateMentorHandler>();
            services.AddScoped<UpdateMentorHandler>();
            services.AddScoped<DeleteMentorHandler>();
            services.AddScoped<GetMentorsHandler>();
            services.AddScoped<GetMentorByIdHandler>();

            // Pour Admins
            services.AddScoped<CreateAdminHandler>();
            services.AddScoped<UpdateAdminHandler>();
            services.AddScoped<DeleteAdminHandler>();
            services.AddScoped<GetAdminsHandler>();
            services.AddScoped<GetAdminByIdHandler>();

            //Pour Phases
            services.AddScoped<CreatePhaseHandler>();
            services.AddScoped<UpdatePhaseHandler>();
            services.AddScoped<DeletePhaseHandler>();
            services.AddScoped<GetPhasesHandler>();
            services.AddScoped<GetPhaseByIdHandler>();

                // Pour Assignment
            services.AddScoped<CreateAssignmentHandler>();
            services.AddScoped<DeactivateAssignmentHandler>();
            services.AddScoped<DeleteAssignmentHandler>();
            services.AddScoped<GetAssignmentsHandler>();
            services.AddScoped<GetAssignmentByIdHandler>();

                // Pour WeekFollowUpHanler
            services.AddScoped<CreateWeeklyFollowUpHandler>();
            services.AddScoped<CompleteFollowUpHandler>();
            services.AddScoped<MarkMissedHandler>();
            services.AddScoped<DeleteWeeklyFollowUpHandler>();
            services.AddScoped<GetWeeklyFollowUpsHandler>();
            services.AddScoped<GetWeeklyFollowUpByIdHandler>();
            services.AddScoped<UpdateWeeklyFollowUpHandler>();

            // Pour feedback
            services.AddScoped<CreateFeedbackHandler>();
            services.AddScoped<UpdateFeedbackHandler>();
            services.AddScoped<DeleteFeedbackHandler>();
            services.AddScoped<GetFeedbacksHandler>();
            services.AddScoped<GetFeedbackByIdHandler>();

            // Pour InternFiles
            services.AddScoped<CreateInternFileHandler>();
            services.AddScoped<DeleteInternFileHandler>();
            services.AddScoped<GetInternFilesHandler>();
            services.AddScoped<GetInternFileByIdHandler>();
            services.AddScoped<UpdateInternFileHandler>();
            services.AddScoped<SetFilePathHandler>();

            // Pour Week
            services.AddScoped<CreateWeekHandler>();
            services.AddScoped<UpdateWeekHandler>();
            services.AddScoped<DeleteWeekHandler>();
            services.AddScoped<GetWeeksHandler>();
            services.AddScoped<GetWeekByIdHandler>();
            
            return services;   

        }
    }
}

