// Services/Repository/PhaseApiService.cs
using System.Net.Http.Json;
using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
using InterManagement.Application.Features.Phases.DTOs;

namespace InterManagement.Client.Services;

public class PhaseApiService
    : BaseApiService<PhaseDto, CreatePhaseDto, UpdatePhaseDto>,
      IPhaseApiService
{
    protected override string ResourcePath => "phase";

    public PhaseApiService(HttpClient httpClient, ILogger<PhaseApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<PhaseDto>> GetByTraineeAsync(int traineeId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<PhaseDto>>(
                $"{ResourcePath}?traineeId={traineeId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching phases for trainee {TraineeId}", traineeId);
            return [];
        }
    }

    public async Task<PhaseDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PhaseDetailDto>(
                $"{ResourcePath}/{id}", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching phase details for id {Id}", id);
            return null;
        }
    }

    public async Task<List<int>?> CreateForMultipleTraineesAsync(
        CreatePhaseForMultipleTraineesDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ResourcePath}/multi-create", dto, JsonOptions);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<int>>(JsonOptions);

            _logger.LogWarning("CreateForMultipleTrainees failed with {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating phase for multiple trainees");
            return null;
        }
    }
}
