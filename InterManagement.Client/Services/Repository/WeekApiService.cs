using System.Net.Http.Json;
using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Client.Services;

public class WeekApiService
    : BaseApiService<WeekDto, CreateWeekDto, UpdateWeekDto>,
      IWeekApiService
{
    protected override string ResourcePath => "week";

    public WeekApiService(HttpClient httpClient, ILogger<WeekApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<WeekDto>> GetByPhaseAsync(int phaseId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WeekDto>>(
                $"{ResourcePath}?phaseId={phaseId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching weeks for phase {PhaseId}", phaseId);
            return [];
        }
    }
}
