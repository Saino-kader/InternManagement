using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Admins.Queries.GetAdmins
{
    public class GetAdminsHandler
    {
        private readonly IAdminRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "admins:all";

        public GetAdminsHandler(IAdminRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<AdminDto>> Handle(GetAdminsQuery query)
        {
            // Vérifie le cache
            if (_cache.TryGetValue(CacheKey, out IEnumerable<AdminDto>? cached) && cached != null)
            {
                return cached;
            }

            // Pas en cache → va chercher en base
            var admins = await _repository.GetAllAsync();
            var result = admins.Select(a => new AdminDto
            {
                Id        = a.Id,
                FirstName = a.FirstName,
                LastName  = a.LastName,
                Email     = a.Email,
                IsActive  = a.IsActive
            }).ToList();

            // Stocke en cache pour 5 minutes
            _cache.Set(CacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}