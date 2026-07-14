using InterManagement.Application.Features.Mentors.DTOs;

namespace InterManagement.Client.Services;

public interface IMentorApiService
    : IBaseApiService<MentorDto, CreateMentorDto, UpdateMentorDto>
{
    Task<List<MentorDto>> GetByDepartmentAsync(string department);
    Task<MentorDetailDto?> GetDetailsAsync(int id);
}