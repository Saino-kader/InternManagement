using InterManagement.Application.Features.Assignments.DTOs;

namespace InterManagement.Client.Services;

public interface IAssignmentApiService : IBaseApiService<AssignmentDto, CreateAssignmentDto, UpdateAssignmentDto>
{
    Task<List<AssignmentDto>> GetByMentorAsync(int mentorId);
    Task<List<AssignmentDto>> GetByTraineeAsync(int traineeId);
    Task<List<MentorAssignmentDetailDto>> GetMentorAssignmentsDetailsAsync(int mentorId);
}
