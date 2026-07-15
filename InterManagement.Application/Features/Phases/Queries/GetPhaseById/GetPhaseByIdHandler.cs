using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Phases.Queries.GetPhaseById
{
    public class GetPhaseByIdHandler
    {
        private readonly IPhaseRepository _repository;

        public GetPhaseByIdHandler(IPhaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<PhaseDetailDto> Handle(GetPhaseByIdQuery query)
        {
            var phase = await _repository
                .GetWithFollowUpsAsync(query.Id);
            if (phase == null)
                throw new PhaseNotFoundException(query.Id);

            return new PhaseDetailDto
            {
                Id          = phase.Id,
                PhaseNumber = phase.PhaseNumber,
                Title       = phase.Title,
                StartDate   = phase.StartDate,
                EndDate     = phase.EndDate,
                Status      = phase.Status,
                TraineeId   = phase.TraineeId,
                Weeks       = phase.Weeks
                    .OrderBy(w => w.WeekNumber)
                    .Select(w => new WeekDto
                    {
                        Id = w.Id,
                        WeekNumber = w.WeekNumber,
                        Course = w.Course,
                        StartDate = w.StartDate,
                        EndDate = w.EndDate,
                        Status = w.Status,
                        PhaseId = w.PhaseId,
                        PhaseTitle = phase.Title
                    })
                    .ToList()
            };
        }
    }
}
