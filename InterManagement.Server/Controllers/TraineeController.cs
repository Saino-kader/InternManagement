using InterManagement.Application.Features.Trainees.Commands.CreateTrainee;
using InterManagement.Application.Features.Trainees.Commands.UpdateTrainee;
using InterManagement.Application.Features.Trainees.Commands.DeleteTrainee;
using InterManagement.Application.Features.Trainees.Queries.GetTrainees;
using InterManagement.Application.Features.Trainees.Queries.GetTraineeById;
using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Domain.Entities;
using InterManagement.Domain.Exceptions;
using InterManagement.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController] 

    [Route("api/[controller]")] 

    public class TraineeController : ControllerBase
        {
        

        private readonly CreateTraineeHandler _createHandler;
        private readonly UpdateTraineeHandler _updateHandler;
        private readonly DeleteTraineeHandler _deleteHandler;
        private readonly GetTraineesHandler _getHandler;
        private readonly GetTraineeByIdHandler _getByIdHandler;


                public TraineeController(
            CreateTraineeHandler createHandler,
            UpdateTraineeHandler updateHandler,
            DeleteTraineeHandler deleteHandler,
            GetTraineesHandler getHandler,
            GetTraineeByIdHandler getByIdHandler)
        {
            _createHandler   = createHandler;
            _updateHandler   = updateHandler;
            _deleteHandler   = deleteHandler;
            _getHandler      = getHandler;
            _getByIdHandler  = getByIdHandler;
        }


       
        [HttpGet] 

        public async Task<IActionResult> GetAll(
            [FromQuery] TraineeStatus? status)
        
        {
            var query  = new GetTraineesQuery { Status = status };

            var result = await _getHandler.Handle(query);

            return Ok(result);          

        }


  
        
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query  = new GetTraineeByIdQuery(id);

                var result = await _getByIdHandler.Handle(query);
                return Ok(result);     

            }
            catch (TraineeNotFoundException ex)
            {
                return NotFound(ex.Message);             

            }
        }


        
        [HttpPost] 
        public async Task<IActionResult> Create(
            [FromBody] CreateTraineeDto dto) 

        {
            try
            {    // 1. Créer la Command avec les données reçues
                var command = new CreateTraineeCommand(dto);

                // 2. Exécuter le Handler (valide, crée, sauvegarde)
                var result  = await _createHandler.Handle(command);

            

                return CreatedAtAction(
                    nameof(GetById),           
                    new { id = result.Id },    
                    result);                   

            }
            catch (TraineeAlreadyExistsException ex)         

            {
                return Conflict(ex.Message);                 }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); // → 400 Bad Request  (champ vide, date invalide) 
            }
        }

        [HttpPut("{id}")] // → paramètre "id" dans l'URL  : PUT /api/trainee/5
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTraineeDto dto)  // Nouvelles données dans le body
        {
            try
            {
                // Créer la Command avec l'Id et les nouvelles données
                var command = new UpdateTraineeCommand(id, dto);

                var result  = await _updateHandler.Handle(command);
                return Ok(result);
            }
            catch (TraineeNotFoundException ex)
            {
                return NotFound(ex.Message); //  Si le stagiaire n'existe pas → 404 Not Found
            }
            catch (TraineeNotActiveException ex)  
            {
                return BadRequest(ex.Message);  // Si le stagiaire est inactif (IsActive = false) → 400 Bad Request
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);  // Si une validation échoue → 400 Bad Request
            }
        }

        // ==================================================
        // MÉTHODE 5 : DELETE supprimer un stagiaire (soft delete)
        // ==================================================       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteTraineeCommand(id); // Créer la Command avec l'Id
                await _deleteHandler.Handle(command);   // Exécuter le Handler → IsDeleted = true
                
                return NoContent();               // Retourner 204 No Content (suppression réussie, pas de données à renvoyer)

            }
            catch (TraineeNotFoundException ex)
            {
                return NotFound(ex.Message);   // Si le stagiaire n'existe pas → 404 Not Found
            }
        }



        // ==================================================
        // MÉTHODE SUPPLEMENTAIRE 6 : GET stagiaires actifs seulement
        // ==================================================
        // Écran Admin — Stagiaires actifs seulement
        // GET api/trainee/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            // Créer la Query avec le filtre Status = InProgress
            var query  = new GetTraineesQuery 
            { 
                Status = TraineeStatus.InProgress 
            };
            var result = await _getHandler.Handle(query);
            return Ok(result);           // Retourner 200 OK avec la liste filtrée

        }


        // ==================================================
        // MÉTHODE SUPPLEMENTAIRE 7 : GET détail complet d'un stagiaire
        // ==================================================
        
        // Écran Mentor — Détail stagiaire avec phases
        // GET api/trainee/5/details
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                     // Créer la Query avec l'Id
                var query  = new GetTraineeByIdQuery(id);
                var result = await _getByIdHandler.Handle(query);
                return Ok(result);  // 200 OK avec le détail complet
            }
            catch (TraineeNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
        }
    }
}
