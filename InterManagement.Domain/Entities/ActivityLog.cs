using InterManagement.Domain.Exceptions;

namespace InterManagement.Domain.Entities
{
    public class ActivityLog : BaseModel
    {
        public string UserName { get; private set; } = string.Empty;
        public string Action { get; private set; } = string.Empty;
        public string Detail { get; private set; } = string.Empty;
        public DateTime OccurredAt { get; private set; }

        private ActivityLog() { }

        public ActivityLog(string userName, string action, string detail)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new DomainException("UserName is required");
            if (string.IsNullOrWhiteSpace(action))
                throw new DomainException("Action is required");

            UserName = userName;
            Action = action;
            Detail = detail;
            OccurredAt = DateTime.UtcNow;
        }
}
}