using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Queries.GetWeeks
{
    public class GetWeeksHandler
    {
        private readonly IWeekRepository _repository;

        public GetWeeksHandler(IWeekRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WeekDto>> Handle(
            GetWeeksQuery query)
        {
            IEnumerable<Week> weeks;

            if (query.PhaseId.HasValue)
                weeks = await _repository
                    .GetByPhaseAsync(query.PhaseId.Value);
            else
                weeks = await _repository.GetAllAsync();

            return weeks.Select(w => new WeekDto
            {
                Id         = w.Id,
                WeekNumber = w.WeekNumber,
                Course     = w.Course,
                StartDate  = w.StartDate,
                EndDate    = w.EndDate,
                Status     = w.Status,
                PhaseId    = w.PhaseId,
                PhaseTitle = w.Phase.Title
            });
        }
    }
}