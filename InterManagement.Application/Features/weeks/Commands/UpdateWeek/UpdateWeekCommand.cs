using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Application.Features.Weeks.Commands.UpdateWeek
{
    public class UpdateWeekCommand
    {
        public int Id { get; set; }
        public UpdateWeekDto Data { get; set; }

        public UpdateWeekCommand(int id, UpdateWeekDto data)
        {
            Id   = id;
            Data = data;
        }
    }
}