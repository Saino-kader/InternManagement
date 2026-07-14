using InterManagement.Application.Common;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Mentors.Commands.DeleteMentor
{
    public class DeleteMentorHandler
    {
        private readonly IMentorRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "mentors:all";

        public DeleteMentorHandler(
            IMentorRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task Handle(DeleteMentorCommand command)
        {
            var mentor = await _repository.GetByIdAsync(command.Id);
            if (mentor == null)
                throw new MentorNotFoundException(command.Id);

            var fullName = $"{mentor.FirstName} {mentor.LastName}";
            await _repository.DeleteAsync(command.Id);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Mentor",
                "Suppression",
                $"{fullName} supprimé"
            );
        }
    }
}
