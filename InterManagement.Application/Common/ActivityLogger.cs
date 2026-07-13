using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Common
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IActivityLogRepository _repository;

        public ActivityLogger(IActivityLogRepository repository)
        {
            _repository = repository;
        }

        public async Task LogAsync(string userName, string action, string detail)
        {
            var log = new ActivityLog(userName, action, detail);
            await _repository.AddAsync(log);
        }
    }
}
