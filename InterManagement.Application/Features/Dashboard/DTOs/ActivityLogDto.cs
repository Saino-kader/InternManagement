namespace InterManagement.Application.Features.Dashboard.DTOs
{
    public class ActivityLogDto
    {
        public DateTime OccurredAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
