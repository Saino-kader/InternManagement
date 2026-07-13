// Services/IRepository/IFeedbackApiService.cs
using InterManagement.Application.Features.Feedbacks.DTOs;

namespace InterManagement.Client.Services;

public interface IFeedbackApiService
    : IBaseApiService<FeedbackDto, CreateFeedbackDto, UpdateFeedbackDto>
{
    // Feedbacks d'un stagiaire précis
    Task<List<FeedbackDto>> GetByTraineeAsync(int traineeId);

    // Feedbacks envoyés par un mentor précis
    // → appelle GET /api/feedback?mentorId=X
    Task<List<FeedbackDto>> GetByMentorAsync(int mentorId);
}
