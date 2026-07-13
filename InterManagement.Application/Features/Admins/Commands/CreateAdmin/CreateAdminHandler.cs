using InterManagement.Application.Common;
using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminHandler
    {
        private readonly IAdminRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "admins:all";

        public CreateAdminHandler(
            IAdminRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task<AdminDto> Handle(CreateAdminCommand command)
        {
            var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists)
                throw new AdminAlreadyExistsException(command.Data.Email);

            var admin = new Admin(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email
            );

            admin.IsActive = command.Data.IsActive;

            await _repository.AddAsync(admin);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Admin",
                "Ajout utilisateur",
                $"{admin.FirstName} {admin.LastName} ajouté"
            );

            return new AdminDto
            {
                Id        = admin.Id,
                FirstName = admin.FirstName,
                LastName  = admin.LastName,
                Email     = admin.Email,
                IsActive  = admin.IsActive
            };
        }
    }
}
