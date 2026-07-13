using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace InterManagement.Application.Features.Trainees.Queries.GetTrainees
{
    public class GetTraineesHandler
    {
        private readonly ITraineeRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "trainees:all";

        public GetTraineesHandler(ITraineeRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<TraineeDto>> Handle(GetTraineesQuery query)
        {
            // Si filtre de statut présent, ne pas utiliser le cache
            if (query.Status.HasValue)
            {
                var filteredTrainees = await _repository.GetAllWithFiltersAsync(query.Status);
                return filteredTrainees.Select(t => new TraineeDto
                {
                    Id         = t.Id,
                    FirstName  = t.FirstName,
                    LastName   = t.LastName,
                    Email      = t.Email,
                    University = t.University,
                    Specialty  = t.Specialty,
                    Theme      = t.Theme,
                    StartDate  = t.StartDate,
                    EndDate    = t.EndDate,
                    Status     = t.Status,
                    IsActive   = t.IsActive
                });
            }

            // Vérifie le cache
            if (_cache.TryGetValue(CacheKey, out IEnumerable<TraineeDto>? cached) && cached != null)
            {
                return cached;
            }

            // Pas en cache → va chercher en base
            var trainees = await _repository.GetAllWithFiltersAsync(null);
            var result = trainees.Select(t => new TraineeDto
            {
                Id         = t.Id,
                FirstName  = t.FirstName,
                LastName   = t.LastName,
                Email      = t.Email,
                University = t.University,
                Specialty  = t.Specialty,
                Theme      = t.Theme,
                StartDate  = t.StartDate,
                EndDate    = t.EndDate,
                Status     = t.Status,
                IsActive   = t.IsActive
            }).ToList();

            // Stocke en cache pour 5 minutes
            _cache.Set(CacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
