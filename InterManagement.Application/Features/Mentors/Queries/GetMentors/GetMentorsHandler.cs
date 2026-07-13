using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Application.Features.Mentors.Queries.GetMentors;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

public class GetMentorsHandler
    {
        private readonly IMentorRepository _repository;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "mentors:all";

        public GetMentorsHandler(IMentorRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<MentorDto>> Handle(GetMentorsQuery query)
        {
            // Si filtre de département présent, ne pas utiliser le cache
            if (!string.IsNullOrWhiteSpace(query.Department))
            {
                var filteredMentors = await _repository.GetByDepartmentAsync(query.Department);
                return filteredMentors.Select(m => new MentorDto
                {
                    Id         = m.Id,
                    FirstName  = m.FirstName,
                    LastName   = m.LastName,
                    Email      = m.Email,
                    Department = m.Department,
                    Specialty  = m.Specialty,
                    IsActive   = m.IsActive,
                    TraineeCount = m.Assignments?.Count(a => a.IsActive) ?? 0
                });
            }

            // Vérifie le cache
            if (_cache.TryGetValue(CacheKey, out IEnumerable<MentorDto>? cached) && cached != null)
            {
                return cached;
            }

            // Pas en cache → va chercher en base
            var mentors = await _repository.GetAllWithTraineeCountAsync();
            var result = mentors.Select(m => new MentorDto
            {
                Id         = m.Id,
                FirstName  = m.FirstName,
                LastName   = m.LastName,
                Email      = m.Email,
                Department = m.Department,
                Specialty  = m.Specialty,
                IsActive   = m.IsActive,
                TraineeCount = m.Assignments?.Count(a => a.IsActive) ?? 0
            }).ToList();

            // Stocke en cache pour 5 minutes
            _cache.Set(CacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
