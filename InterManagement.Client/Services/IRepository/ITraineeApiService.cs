using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Shared.Enums;

namespace InterManagement.Client.Services;

public interface ITraineeApiService
    : IBaseApiService<TraineeDto, CreateTraineeDto, UpdateTraineeDto>
{
    Task<List<TraineeDto>> GetActiveAsync();
    Task<List<TraineeDto>> GetByStatusAsync(TraineeStatus status);
    Task<TraineeDetailDto?> GetDetailsAsync(int id);
}
