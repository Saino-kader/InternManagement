using InterManagement.Application.Features.Assignments.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
//using Microsoft.EntityFrameworkCore;

namespace InterManagement.Application.Features.Assignments.Queries.GetMentorAssignments
{
    public class GetMentorAssignmentsHandler
    {
        private readonly IAssignmentRepository _repository;

        public GetMentorAssignmentsHandler(IAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MentorAssignmentDetailDto>> Handle(GetMentorAssignmentsQuery query)
        {
            // Récupérer les assignments avec toutes les relations chargées
            var assignments = await _repository.GetByMentorAsync(query.MentorId);

            var result = new List<MentorAssignmentDetailDto>();

            foreach (var assignment in assignments)
            {
                var dto = new MentorAssignmentDetailDto
                {
                    Id = assignment.Id,
                    AssignmentDate = assignment.AssignmentDate,
                    IsActive = assignment.IsActive,
                    TraineeId = assignment.TraineeId,
                    TraineeName = $"{assignment.Trainee.FirstName} {assignment.Trainee.LastName}",
                    MentorId = assignment.MentorId,
                    MentorName = $"{assignment.Mentor.FirstName} {assignment.Mentor.LastName}",
                    PhaseId = assignment.PhaseId,
                    PhaseTitle = assignment.Phase.Title,
                    PhaseNumber = assignment.Phase.PhaseNumber,
                    PhaseStatus = assignment.Phase.Status.ToString()
                };

                // Ajouter les semaines de la phase avec leurs WeeklyFollowUps
                foreach (var week in assignment.Phase.Weeks.OrderBy(w => w.WeekNumber))
                {
                    // Chercher le WeeklyFollowUp pour ce stagiaire et cette semaine
                    var weeklyFollowUp = week.WeeklyFollowUps
                        .FirstOrDefault(wf => wf.TraineeId == assignment.TraineeId);

                    dto.Weeks.Add(new MentorWeekItemDto
                    {
                        Id = week.Id,
                        WeekNumber = week.WeekNumber,
                        Course = week.Course,
                        StartDate = week.StartDate,
                        EndDate = week.EndDate,
                        Status = weeklyFollowUp?.Status.ToString()
                    });
                }

                result.Add(dto);
            }

            return result;
        }
    }
}
