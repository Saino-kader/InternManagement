// Services/Repository/ImportedFollowUpApiService.cs
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace InterManagement.Client.Services;

public class ImportedFollowUpApiService : IImportedFollowUpApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ImportedFollowUpApiService> _logger;

    private const string ResourcePath = "importedfollowup";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ImportedFollowUpApiService(
        HttpClient httpClient,
        ILogger<ImportedFollowUpApiService> logger)
    {
        _httpClient = httpClient;
        _logger     = logger;
    }

    public async Task<List<ImportedFollowUpDto>> GetAllAsync()
    {
        try
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<ImportedFollowUpDto>>(ResourcePath, JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching imported followups");
            return [];
        }
    }

    public async Task<ImportedFollowUpResultDto?> ImportAsync(
        Stream fileStream, string fileName)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient
                .PostAsync($"{ResourcePath}/import", content);

            if (response.IsSuccessStatusCode)
                return await response.Content
                    .ReadFromJsonAsync<ImportedFollowUpResultDto>(JsonOptions);

            _logger.LogWarning("Import failed: {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing file");
            return null;
        }
    }

    public async Task<ImportedFollowUpDto?> UpdateAsync(
        int id, UpdateImportedFollowUpDto dto)
    {
        try
        {
            var response = await _httpClient
                .PutAsJsonAsync($"{ResourcePath}/{id}", dto, JsonOptions);

            if (response.IsSuccessStatusCode)
                return await response.Content
                    .ReadFromJsonAsync<ImportedFollowUpDto>(JsonOptions);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating imported followup {Id}", id);
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient
                .DeleteAsync($"{ResourcePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting imported followup {Id}", id);
            return false;
        }
    }
}
