using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

namespace InterManagement.Application.Features.WeeklyFollowUps.Commands.UpdateWeeklyFollowUp
{
    public class UpdateWeeklyFollowUpCommand
    {
        public int Id { get; set; }
        public UpdateWeeklyFollowUpDto Data { get; set; }

        public UpdateWeeklyFollowUpCommand(int id, UpdateWeeklyFollowUpDto data)
        {
            Id   = id;
            Data = data;
        }
    }
}