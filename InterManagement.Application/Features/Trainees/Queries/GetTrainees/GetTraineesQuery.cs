using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;

namespace InterManagement.Application.Features.Trainees.Queries.GetTrainees
{
    public class GetTraineesQuery
    {
        public TraineeStatus? Status { get; set; }
    }
}
