using InterManagement.Application.Common;
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Trainees.Commands.UpdateTrainee
{
    public class UpdateTraineeHandler
    {
        private readonly ITraineeRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "trainees:all";

        public UpdateTraineeHandler(
            ITraineeRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task<TraineeDto> Handle(UpdateTraineeCommand command)
        {
            var trainee = await _repository.GetByIdAsync(command.Id);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Id);

              var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists && trainee.Email != command.Data.Email)
                throw new TraineeAlreadyExistsException(command.Data.Email);

            trainee.Update(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email,
                command.Data.University,
                command.Data.Specialty,
                command.Data.Theme,
                command.Data.StartDate,
                command.Data.EndDate,
                command.Data.Status
            );

            trainee.IsActive = command.Data.IsActive;

            await _repository.UpdateAsync(trainee);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Stagiaire",
                "Modification",
                $"{trainee.FirstName} {trainee.LastName} modifié"
            );

            return new TraineeDto
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
