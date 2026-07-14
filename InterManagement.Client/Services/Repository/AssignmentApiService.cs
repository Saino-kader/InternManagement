// Services/Repository/AssignmentApiService.cs
using InterManagement.Application.Features.Assignments.DTOs;
using System.Net.Http.Json;

namespace InterManagement.Client.Services;

public class AssignmentApiService
    : BaseApiService<AssignmentDto, CreateAssignmentDto, UpdateAssignmentDto>,
      IAssignmentApiService
{
    protected override string ResourcePath => "assignment";

    public AssignmentApiService(HttpClient httpClient, ILogger<AssignmentApiService> logger)
        : base(httpClient, logger) { }

    // GET /api/assignment?mentorId=X
    public async Task<List<AssignmentDto>> GetByMentorAsync(int mentorId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<AssignmentDto>>(
                $"{ResourcePath}?mentorId={mentorId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments for mentor {MentorId}", mentorId);
            return [];
        }
    }

    // GET /api/assignment?traineeId=X
    public async Task<List<AssignmentDto>> GetByTraineeAsync(int traineeId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<AssignmentDto>>(
                $"{ResourcePath}?traineeId={traineeId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching assignments for trainee {TraineeId}", traineeId);
            return [];
        }
    }

    public async Task<List<MentorAssignmentDetailDto>> GetMentorAssignmentsDetailsAsync(int mentorId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MentorAssignmentDetailDto>>(
                $"{ResourcePath}/mentor/{mentorId}/details", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching mentor assignment details for mentor {MentorId}", mentorId);
            return [];
        }
    }
}