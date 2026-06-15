using InterManagement.Application.Features.Weeks.DTOs;

namespace InterManagement.Application.Features.Weeks.Commands.CreateWeek
{
    public class CreateWeekCommand
    {
        public CreateWeekDto Data { get; set; }

        public CreateWeekCommand(CreateWeekDto data)
        {
            Data = data;
        }
    }
}