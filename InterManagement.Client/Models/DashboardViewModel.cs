// Client/Models/DashboardViewModel.cs
using InterManagement.Application.Features.Dashboard.DTOs;

namespace InterManagement.Client.Models
{
    public class DashboardViewModel
    {
        public DashboardStatsDto Stats { get; set; } = new();
        public List<ActivityLogDto> RecentActivities { get; set; } = [];
    }
}
