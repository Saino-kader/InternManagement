// Application/Features/Assignments/Queries/GetAssignmentsByMentor/GetAssignmentsByMentorHandler.cs
using InterManagement.Application.Features.Assignments.DTOs;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Assignments.Queries.GetAssignmentsByMentor
{
    public class GetAssignmentsByMentorHandler
    {
        private readonly IAssignmentRepository _repository;

        public GetAssignmentsByMentorHandler(IAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AssignmentDto>> Handle(
            GetAssignmentsByMentorQuery query)
        {
            // Récupère tous les assignments de ce mentor
            // avec leurs relations Trainee + Phase chargées
            var assignments = await _repository.GetByMentorAsync(query.MentorId);

            return assignments.Select(a => new AssignmentDto
            {
                Id             = a.Id,
                TraineeId      = a.TraineeId,
                TraineeName    = $"{a.Trainee.FirstName} {a.Trainee.LastName}",
                MentorId       = a.MentorId,
                MentorName     = $"{a.Mentor.FirstName} {a.Mentor.LastName}",
                PhaseId        = a.PhaseId,
                PhaseTitle     = a.Phase.Title,
                PhaseNumber    = a.Phase.PhaseNumber,
                IsActive       = a.IsActive,
                AssignmentDate = a.AssignmentDate
            });
        }
    }
}