using InterManagement.Application.Features.Dashboard.DTOs;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Dashboard.Queries.GetRecentActivity
{
    public class GetRecentActivityHandler
    {
        private readonly IActivityLogRepository _repository;

        public GetRecentActivityHandler(IActivityLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ActivityLogDto>> Handle(GetRecentActivityQuery query)
        {
            var logs = await _repository.GetRecentAsync(query.Count);
            return logs.Select(l => new ActivityLogDto
            {
                OccurredAt = l.OccurredAt,
                UserName = l.UserName,
                Action = l.Action,
                Detail = l.Detail
            });
        }
    }
}
