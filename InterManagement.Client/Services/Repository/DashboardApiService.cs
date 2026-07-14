// Services/Repository/DashboardApiService.cs
using System.Net.Http.Json;
using System.Text.Json;
using InterManagement.Application.Features.Dashboard.DTOs;

namespace InterManagement.Client.Services;

public class DashboardApiService : IDashboardApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DashboardApiService(HttpClient httpClient, ILogger<DashboardApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DashboardStatsDto?> GetStatsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DashboardStatsDto>(
                "dashboard/stats", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard stats");
            return null;
        }
    }

    public async Task<List<ActivityLogDto>> GetRecentActivityAsync(int count = 10)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ActivityLogDto>>(
                $"dashboard/recent-activity?count={count}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent activity");
            return [];
        }
    }
}
