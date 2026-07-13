using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUps
{
    public class GetWeeklyFollowUpsHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;

        public GetWeeklyFollowUpsHandler(
            IWeeklyFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<WeeklyFollowUpDto>> Handle(
            GetWeeklyFollowUpsQuery query)
        {
            IEnumerable<WeeklyFollowUp> followUps;

            if (query.PhaseId.HasValue)
                followUps = await _repository
                    .GetByPhaseAsync(query.PhaseId.Value);
            else if (query.MentorId.HasValue)
                followUps = await _repository
                    .GetByMentorAsync(query.MentorId.Value);
            else
                followUps = await _repository.GetAllAsync();

            return followUps.Select(w => new WeeklyFollowUpDto
            {
                Id           = w.Id,
                FollowUpDate = w.FollowUpDate,
                Status       = w.Status,
                Comment      = w.Comment,
                CourseName   = w.CourseName,
                Appreciation = w.Appreciation,
                WeekNumber   = w.Week.WeekNumber,
                WeekId       = w.WeekId,
                TraineeId    = w.TraineeId,
                TraineeName  = $"{w.Trainee.FirstName} {w.Trainee.LastName}",
                MentorId     = w.MentorId,
                MentorName   = $"{w.Mentor.FirstName} {w.Mentor.LastName}"
            });
        }
    }
}
