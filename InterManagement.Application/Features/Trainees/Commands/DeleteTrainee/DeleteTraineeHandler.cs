using InterManagement.Application.Common;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Trainees.Commands.DeleteTrainee
{
    public class DeleteTraineeHandler
    {
        private readonly ITraineeRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "trainees:all";

        public DeleteTraineeHandler(
            ITraineeRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task Handle(DeleteTraineeCommand command)
        {
            var trainee = await _repository.GetByIdAsync(command.Id);
            if (trainee == null)
                throw new TraineeNotFoundException(command.Id);

            var fullName = $"{trainee.FirstName} {trainee.LastName}";
            await _repository.DeleteAsync(command.Id);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Stagiaire",
                "Suppression",
                $"{fullName} supprimé"
            );
        }
    }
}
