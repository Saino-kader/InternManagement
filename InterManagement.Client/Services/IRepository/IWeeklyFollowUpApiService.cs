using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.WeeklyFollowUps.Commands.ImportWeeklyFollowUps;

namespace InterManagement.Client.Services;

public interface IWeeklyFollowUpApiService
    : IBaseApiService<WeeklyFollowUpDto, CreateWeeklyFollowUpDto, UpdateWeeklyFollowUpDto>
{
    Task<List<WeeklyFollowUpDto>> GetByWeekAsync(int weekId);
    Task<List<WeeklyFollowUpDto>> GetByMentorAsync(int mentorId);
    Task<bool> CompleteAsync(int id, string comment);
    Task<bool> MarkMissedAsync(int id);
    Task<ImportResultDto?> ImportAsync(Stream fileStream, string fileName);
    Task<(byte[] fileBytes, string fileName)?> ExportByTraineeAsync(int traineeId);
}
