using System.Net.Http.Json;
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;

namespace InterManagement.Client.Services;

public class WeeklyFollowUpApiService
    : BaseApiService<WeeklyFollowUpDto, CreateWeeklyFollowUpDto, UpdateWeeklyFollowUpDto>,
      IWeeklyFollowUpApiService
{
    protected override string ResourcePath => "weeklyfollowup";

    public WeeklyFollowUpApiService(HttpClient httpClient, ILogger<WeeklyFollowUpApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<WeeklyFollowUpDto>> GetByWeekAsync(int weekId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WeeklyFollowUpDto>>(
                $"{ResourcePath}?weekId={weekId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching follow-ups for week {WeekId}", weekId);
            return [];
        }
    }

    public async Task<List<WeeklyFollowUpDto>> GetByMentorAsync(int mentorId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WeeklyFollowUpDto>>(
                $"{ResourcePath}?mentorId={mentorId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching follow-ups for mentor {MentorId}", mentorId);
            return [];
        }
    }

    public async Task<bool> CompleteAsync(int id, string comment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"{ResourcePath}/{id}/complete", comment, JsonOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing follow-up {Id}", id);
            return false;
        }
    }

    public async Task<bool> MarkMissedAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync(
                $"{ResourcePath}/{id}/missed", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking follow-up {Id} as missed", id);
            return false;
        }
    }

    public async Task<ImportResultDto?> ImportAsync(Stream fileStream, string fileName)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            form.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync(
                $"{ResourcePath}/import", form);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Import failed with {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ImportResultDto>(JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing follow-ups from file {FileName}", fileName);
            return null;
        }
    }

    public async Task<(byte[] fileBytes, string fileName)?> ExportByTraineeAsync(int traineeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ResourcePath}/export/{traineeId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Export failed with {StatusCode}", response.StatusCode);
                return null;
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = response.Content.Headers.ContentDisposition?.FileName
                ?? $"suivi_export_{traineeId}.xlsx";

            return (bytes, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting follow-ups for trainee {TraineeId}", traineeId);
            return null;
        }
    }
}
