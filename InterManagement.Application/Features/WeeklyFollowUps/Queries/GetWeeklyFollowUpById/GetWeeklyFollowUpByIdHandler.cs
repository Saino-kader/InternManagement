using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.WeeklyFollowUps.Queries.GetWeeklyFollowUpById
{
    public class GetWeeklyFollowUpByIdHandler
    {
        private readonly IWeeklyFollowUpRepository _repository;

        public GetWeeklyFollowUpByIdHandler(
            IWeeklyFollowUpRepository repository)
        {
            _repository = repository;
        }

        public async Task<WeeklyFollowUpDto> Handle(
            GetWeeklyFollowUpByIdQuery query)
        {
            var followUp = await _repository.GetByIdAsync(query.Id);
            if (followUp == null)
                throw new WeeklyFollowUpNotFoundException(query.Id);

            return new WeeklyFollowUpDto
            {
                Id           = followUp.Id,
                FollowUpDate = followUp.FollowUpDate,
                Status       = followUp.Status,
                Comment      = followUp.Comment,
                CourseName   = followUp.CourseName,
                Appreciation = followUp.Appreciation,
                WeekNumber   = followUp.Week.WeekNumber,
                WeekId       = followUp.WeekId,
                TraineeId    = followUp.TraineeId,
                TraineeName  = $"{followUp.Trainee.FirstName} {followUp.Trainee.LastName}",
                MentorId     = followUp.MentorId,
                MentorName   = $"{followUp.Mentor.FirstName} {followUp.Mentor.LastName}"
            };
        }
    }
}
