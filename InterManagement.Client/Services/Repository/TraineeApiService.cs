// Services/Repository/TraineeApiService.cs
using System.Net.Http.Json;
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Shared.Enums;

namespace InterManagement.Client.Services;

public class TraineeApiService
    : BaseApiService<TraineeDto, CreateTraineeDto, UpdateTraineeDto>,
      ITraineeApiService
{
    protected override string ResourcePath => "trainee";

    public TraineeApiService(HttpClient httpClient, ILogger<TraineeApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<TraineeDto>> GetActiveAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}/active", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active trainees");
            return [];
        }
    }

    public async Task<List<TraineeDto>> GetByStatusAsync(TraineeStatus status)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<TraineeDto>>(
                $"{ResourcePath}?status={(int)status}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainees by status {Status}", status);
            return [];
        }
    }

    public async Task<TraineeDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TraineeDetailDto>(
                $"{ResourcePath}/{id}/details", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching trainee details for id {Id}", id);
            return null;
        }
    }
}
