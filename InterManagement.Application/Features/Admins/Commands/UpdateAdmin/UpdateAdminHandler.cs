using InterManagement.Application.Common;
using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Admins.Commands.UpdateAdmin
{
    public class UpdateAdminHandler
    {
        private readonly IAdminRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "admins:all";

        public UpdateAdminHandler(
            IAdminRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task<AdminDto> Handle(UpdateAdminCommand command)
        {
            var admin = await _repository.GetByIdAsync(command.Id);
            if (admin == null)
                throw new AdminNotFoundException(command.Id);

            if (!admin.IsActive)
                throw new AdminNotActiveException(command.Id);

            var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists && admin.Email != command.Data.Email)
                throw new AdminAlreadyExistsException(command.Data.Email);

            admin.Update(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email
            );

            admin.IsActive = command.Data.IsActive;

            await _repository.UpdateAsync(admin);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Admin",
                "Modification",
                $"{admin.FirstName} {admin.LastName} modifié"
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
