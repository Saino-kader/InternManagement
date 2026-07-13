using InterManagement.Application.Common;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Admins.Commands.DeleteAdmin
{
    public class DeleteAdminHandler
    {
        private readonly IAdminRepository _repository;
        private readonly IActivityLogger _activityLogger;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "admins:all";

        public DeleteAdminHandler(
            IAdminRepository repository,
            IActivityLogger activityLogger,
            IMemoryCache cache)
        {
            _repository = repository;
            _activityLogger = activityLogger;
            _cache = cache;
        }

        public async Task Handle(DeleteAdminCommand command)
        {
            var admin = await _repository.GetByIdAsync(command.Id);
            if (admin == null)
                throw new AdminNotFoundException(command.Id);

            var fullName = $"{admin.FirstName} {admin.LastName}";
            await _repository.DeleteAsync(command.Id);

            // Invalide le cache
            _cache.Remove(CacheKey);

            await _activityLogger.LogAsync(
                "Admin",
                "Suppression",
                $"{fullName} supprimé"
            );
        }
    }
}
