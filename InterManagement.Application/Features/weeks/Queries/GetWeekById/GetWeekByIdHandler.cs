using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Weeks.Queries.GetWeekById
{
    public class GetWeekByIdHandler
    {
        private readonly IWeekRepository _repository;

        public GetWeekByIdHandler(IWeekRepository repository)
        {
            _repository = repository;
        }

        public async Task<WeekDto> Handle(GetWeekByIdQuery query)
        {
            var week = await _repository.GetByIdAsync(query.Id);
            if (week == null)
                throw new WeekNotFoundException(query.Id);

            return new WeekDto
            {
                Id         = week.Id,
                WeekNumber = week.WeekNumber,
                Course     = week.Course,
                StartDate  = week.StartDate,
                EndDate    = week.EndDate,
                Status     = week.Status,
                PhaseId    = week.PhaseId,
                PhaseTitle = week.Phase.Title
            };
        }
    }
}