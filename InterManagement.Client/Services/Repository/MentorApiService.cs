using InterManagement.Application.Features.Mentors.DTOs;
using System.Net.Http.Json;

namespace InterManagement.Client.Services;

public class MentorApiService
    : BaseApiService<MentorDto, CreateMentorDto, UpdateMentorDto>,
      IMentorApiService
{
    protected override string ResourcePath => "mentor";

    public MentorApiService(HttpClient httpClient, ILogger<MentorApiService> logger)
        : base(httpClient, logger) { }

    public async Task<List<MentorDto>> GetByDepartmentAsync(string department)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MentorDto>>(
                $"{ResourcePath}?department={department}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching mentors by department {Department}", department);
            return [];
        }
    }

    public async Task<MentorDetailDto?> GetDetailsAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<MentorDetailDto>(
                $"{ResourcePath}/{id}/details", JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching mentor details for id {Id}", id);
            return null;
        }
    }
}
