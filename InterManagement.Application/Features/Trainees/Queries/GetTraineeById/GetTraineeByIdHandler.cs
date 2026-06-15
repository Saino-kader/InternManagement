using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Trainees.Queries.GetTraineeById
{
    public class GetTraineeByIdHandler
    {
        private readonly ITraineeRepository _repository;

        public GetTraineeByIdHandler(ITraineeRepository repository)
        {
            _repository = repository;
        }

        public async Task<TraineeDetailDto> Handle(GetTraineeByIdQuery query)
        {
            // 1. Chercher avec ses phases
            var trainee = await _repository.GetByIdAsync(query.Id);
            if (trainee == null)
                throw new TraineeNotFoundException(query.Id);

            // 2. Retourner le DTO détaillé
            return new TraineeDetailDto
            {
                Id         = trainee.Id,
                FirstName  = trainee.FirstName,
                LastName   = trainee.LastName,
                Email      = trainee.Email,
                University = trainee.University,
                Specialty  = trainee.Specialty,
                Theme      = trainee.Theme,
                StartDate  = trainee.StartDate,
                EndDate    = trainee.EndDate,
                Status     = trainee.Status,
                IsActive   = trainee.IsActive
            };
        }
    }
}
