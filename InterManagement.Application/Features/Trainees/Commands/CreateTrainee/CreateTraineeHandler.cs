using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Shared.Enums;
using InterManagement.Domain.Exceptions;
using InterManagement.Domain.Repositories;

namespace InterManagement.Application.Features.Trainees.Commands.CreateTrainee
{


    public class CreateTraineeHandler
    {

        private readonly ITraineeRepository _repository;

        // ITraineeRepository repository → paramètre reçu (injection de dépendances)
        public CreateTraineeHandler(ITraineeRepository repository)
        {
            // Stocke le repository reçu dans le champ _repository
            // Le Handler pourra maintenant appeler _repository.EmailExistsAsync()
            _repository = repository;
        }


        // ==================================================
        // MÉTHODE PRINCIPALE : Handle
        // ==================================================
        
        public async Task<TraineeDto> Handle(CreateTraineeCommand command)

        {

            // ==============================================
            // ÉTAPE 1 : VÉRIFIER QUE L'EMAIL EST UNIQUE
            // ==============================================
            
         
            // 1. Vérifier email existe déjà
            var emailExists = await _repository.EmailExistsAsync(command.Data.Email);
            if (emailExists)
                throw new TraineeAlreadyExistsException(command.Data.Email);

            // ==============================================
            // ÉTAPE 2 : CRÉER L'ENTITÉ TRAINEE
            // ==============================================
            
  
            // 2. Créer l'entité
            var trainee = new Trainee(
                command.Data.FirstName,
                command.Data.LastName,
                command.Data.Email,
                command.Data.University,
                command.Data.Specialty,
                command.Data.Theme,
                command.Data.StartDate,
                command.Data.EndDate,
                TraineeStatus.InProgress
                
            );    //  "Je crée un stagiaire en mémoire pour l'envoyer à la base"
            // À ce stade : trainee existe en mémoire, Id = 0 (pas encore en base)


            // 3. Sauvegarder en base
            // Appelle le repository pour sauvegarder l'entité
            await _repository.AddAsync(trainee);
            
            // ==============================================
            // ÉTAPE 4 : RETOURNER LE DTO AU CLIENT
            // ==============================================
            
          
            return new TraineeDto    
            {
                Id        = trainee.Id,
                FirstName = trainee.FirstName,  
                LastName  = trainee.LastName,
                Email     = trainee.Email,
                University = trainee.University,
                Specialty  = trainee.Specialty,
                Theme      = trainee.Theme,
                StartDate  = trainee.StartDate,
                EndDate    = trainee.EndDate,
                Status     = trainee.Status,
                IsActive   = trainee.IsActive   
            }; // "Je crée une réponse propre à renvoyer au client"       
        }
    }
}

