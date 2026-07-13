using InterManagement.Application.Features.Dashboard.DTOs;

namespace InterManagement.Client.Services;

public interface IDashboardApiService
{
    Task<DashboardStatsDto?> GetStatsAsync();
    Task<List<ActivityLogDto>> GetRecentActivityAsync(int count = 10);
}