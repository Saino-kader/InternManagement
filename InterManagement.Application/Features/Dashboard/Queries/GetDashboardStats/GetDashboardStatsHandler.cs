using InterManagement.Application.Features.Dashboard.DTOs;
using InterManagement.Domain.Repositories;
using InterManagement.Shared.Enums;


namespace InterManagement.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsHandler
    {
        private readonly ITraineeRepository _traineeRepository;
        private readonly IMentorRepository _mentorRepository;

        public GetDashboardStatsHandler(
            ITraineeRepository traineeRepository,
            IMentorRepository mentorRepository)
        {
            _traineeRepository = traineeRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery query)
        {
            return new DashboardStatsDto
            {
                ActiveTraineesCount = await _traineeRepository
                    .CountByStatusAsync(TraineeStatus.InProgress),
                CompletedTraineesCount = await _traineeRepository
                    .CountByStatusAsync(TraineeStatus.Completed),
                ActiveMentorsCount = await _mentorRepository.CountActiveAsync()
            };
        }
    }
}
