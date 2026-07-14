using InterManagement.Application.Common;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Mentors.Commands.UpdateMentor
{
    public class UpdateMentorHandler
    {
        private readonly IMentorRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "mentors:all";

        public UpdateMentorHandler(
            IMentorRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task<MentorDto> Handle(UpdateMentorCommand command)
        {
            var mentor = await _repository.GetByIdAsync(command.Id);
            if (mentor == null)
                throw new MentorNotFoundException(command.Id);

            if (!mentor.IsActive)
                throw new MentorNotActiveException(command.Id);

            var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists && mentor.Email != command.Data.Email)
                throw new MentorAlreadyExistsException(command.Data.Email);

            mentor.Update(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email,
                command.Data.Department,
                command.Data.Specialty
            );

            mentor.IsActive = command.Data.IsActive;

            await _repository.UpdateAsync(mentor);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Mentor",
                "Modification",
                $"{mentor.FirstName} {mentor.LastName} modifié"
            );

            return new MentorDto
            {
                Id         = mentor.Id,
                FirstName  = mentor.FirstName,
                LastName   = mentor.LastName,
                Email      = mentor.Email,
                Department = mentor.Department,
                Specialty  = mentor.Specialty,
                IsActive   = mentor.IsActive
            };
        }
    }
}
