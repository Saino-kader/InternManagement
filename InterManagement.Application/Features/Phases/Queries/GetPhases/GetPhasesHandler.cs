using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Phases.Queries.GetPhases
{
    public class GetPhasesHandler
    {
        private readonly IPhaseRepository _repository;

        public GetPhasesHandler(IPhaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PhaseDto>> Handle(GetPhasesQuery query)
        {
            IEnumerable<Phase> phases;

            if (query.TraineeId.HasValue)
                phases = await _repository
                    .GetByTraineeAsync(query.TraineeId.Value);
            else
                phases = await _repository.GetAllAsync();

            return phases.Select(p => new PhaseDto
            {
                Id          = p.Id,
                PhaseNumber = p.PhaseNumber,
                Title       = p.Title,
                StartDate   = p.StartDate,
                EndDate     = p.EndDate,
                Status      = p.Status,
                TraineeId   = p.TraineeId,
                MentorId    = p.Assignments
                    .OrderByDescending(a => a.AssignmentDate)
                    .Select(a => (int?)a.MentorId)
                    .FirstOrDefault(),
                MentorName  = p.Assignments
                    .OrderByDescending(a => a.AssignmentDate)
                    .Select(a => a.Mentor.FirstName + " " + a.Mentor.LastName)
                    .FirstOrDefault() ?? string.Empty
                
            });
        }
    }
}