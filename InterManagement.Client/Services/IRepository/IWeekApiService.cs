using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Client.Services;

public interface IWeekApiService
    : IBaseApiService<WeekDto, CreateWeekDto, UpdateWeekDto>
{
    Task<List<WeekDto>> GetByPhaseAsync(int phaseId);
}
