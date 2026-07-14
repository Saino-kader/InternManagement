using InterManagement.Application.Common;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Mentors.Commands.CreateMentor
{
    public class CreateMentorHandler
    {
        private readonly IMentorRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "mentors:all";

        public CreateMentorHandler(
            IMentorRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task<MentorDto> Handle(CreateMentorCommand command)
        {
            var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists)
                throw new MentorAlreadyExistsException(command.Data.Email);

            var mentor = new Mentor(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email,
                command.Data.Department,
                command.Data.Specialty
            );

            mentor.IsActive = command.Data.IsActive;

            await _repository.AddAsync(mentor);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Mentor",
                "Ajout utilisateur",
                $"{mentor.FirstName} {mentor.LastName} ajouté"
            );

            return new MentorDto
            {
                Id           = mentor.Id,
                FirstName    = mentor.FirstName,
                LastName     = mentor.LastName,
                Email        = mentor.Email,
                Department   = mentor.Department,
                Specialty    = mentor.Specialty,
                IsActive     = mentor.IsActive,
                TraineeCount = 0
            };
        }
    }
}
