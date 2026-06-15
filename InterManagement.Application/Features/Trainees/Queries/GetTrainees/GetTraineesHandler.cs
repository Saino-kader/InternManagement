using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Trainees.Queries.GetTrainees
{
    public class GetTraineesHandler
    {
        private readonly ITraineeRepository _repository;

        public GetTraineesHandler(ITraineeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TraineeDto>> Handle(GetTraineesQuery query)
        {
            // 1. Lire depuis la base avec filtre optionnel
            var trainees = await _repository.GetAllWithFiltersAsync(query.Status);

            // 2. Transformer en DTOs et retourner
            return trainees.Select(t => new TraineeDto     // .Select(...): Pour CHAQUE élément de la liste, applique la transformation ou // "Convertir chaque entité en DTO"
            {                                              // t =>	Chaque élément individuel (t = un stagiaire)
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
    }
}
