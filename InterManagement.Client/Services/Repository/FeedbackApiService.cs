// Services/Repository/FeedbackApiService.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using System.Net.Http.Json;

namespace InterManagement.Client.Services;

public class FeedbackApiService
    : BaseApiService<FeedbackDto, CreateFeedbackDto, UpdateFeedbackDto>,
      IFeedbackApiService
{
    protected override string ResourcePath => "feedback";

    public FeedbackApiService(HttpClient httpClient, ILogger<FeedbackApiService> logger)
        : base(httpClient, logger) { }

    // GET /api/feedback?traineeId=X
    public async Task<List<FeedbackDto>> GetByTraineeAsync(int traineeId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<FeedbackDto>>(
                $"{ResourcePath}?traineeId={traineeId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching feedbacks for trainee {TraineeId}", traineeId);
            return [];
        }
    }

    // GET /api/feedback?mentorId=X
    public async Task<List<FeedbackDto>> GetByMentorAsync(int mentorId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<FeedbackDto>>(
                $"{ResourcePath}?mentorId={mentorId}", JsonOptions);
            return result ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching feedbacks for mentor {MentorId}", mentorId);
            return [];
        }
    }
}