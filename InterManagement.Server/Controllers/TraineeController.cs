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



































































































































































/*

// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Importe les Handlers des Commands (écriture)
using InterManagement.Application.Features.Trainees.Commands.CreateTrainee;
using InterManagement.Application.Features.Trainees.Commands.UpdateTrainee;
using InterManagement.Application.Features.Trainees.Commands.DeleteTrainee;

// Importe les Handlers des Queries (lecture)
using InterManagement.Application.Features.Trainees.Queries.GetTrainees;
using InterManagement.Application.Features.Trainees.Queries.GetTraineeById;

// Importe les DTOs (ce qui entre et sort de l'API)
using InterManagement.Application.Features.Trainees.DTOs;

// Importe les entités et exceptions du Domain
using InterManagement.Domain.Entities;      // Pour TraineeStatus
using InterManagement.Domain.Exceptions;    // Pour les exceptions personnalisées

// Importe les outils ASP.NET Core pour les contrôleurs API
using Microsoft.AspNetCore.Mvc;            // ControllerBase, ApiController, Route, etc.

// ======================================================
// NAMESPACE
// ======================================================

// Déclare l'espace de noms (dossier) où se trouve ce fichier
// Correspond au chemin : Server/Controllers/
namespace InterManagement.Server.Controllers;

// ======================================================
// CLASSE CONTROLLER POUR L'API TRAINEE
// ======================================================

// [ApiController] → Attribut qui indique que cette classe est un contrôleur API
//   - Active la validation automatique des modèles
//   - Bind source inference (paramètres viennent de l'URL, body, etc.)
//   - Retourne automatiquement 400 en cas d'erreur de validation
[ApiController]

// [Route("api/[controller]")] → Définit le chemin d'accès à l'API
//   - "api" → préfixe standard pour les APIs
//   - "[controller]" → remplacé par le nom de la classe sans "Controller"
//   - TraineeController → "trainee"
//   - URL finale : /api/trainee
[Route("api/[controller]")]
public class TraineeController : ControllerBase
{
    // ==================================================
    // CHAMPS (VARIABLES DE CLASSE)
    // ==================================================
    
    // private readonly → accessible uniquement dans cette classe, ne peut être modifié
    // Chaque champ stocke un Handler qui sera injecté dans le constructeur
    private readonly CreateTraineeHandler _createHandler;   // Pour créer
    private readonly UpdateTraineeHandler _updateHandler;   // Pour modifier
    private readonly DeleteTraineeHandler _deleteHandler;   // Pour supprimer
    private readonly GetTraineesHandler _getHandler;        // Pour lister
    private readonly GetTraineeByIdHandler _getByIdHandler; // Pour le détail

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // Le constructeur reçoit TOUS les Handlers par injection de dépendances
    // C'est le conteneur DI qui fournit automatiquement ces instances
    // Chaque Handler est enregistré avec AddScoped dans DependencyInjection.cs
    public TraineeController(
        CreateTraineeHandler createHandler,
        UpdateTraineeHandler updateHandler,
        DeleteTraineeHandler deleteHandler,
        GetTraineesHandler getHandler,
        GetTraineeByIdHandler getByIdHandler)
    {
        // On stocke chaque Handler reçu dans le champ correspondant
        _createHandler   = createHandler;
        _updateHandler   = updateHandler;
        _deleteHandler   = deleteHandler;
        _getHandler      = getHandler;
        _getByIdHandler  = getByIdHandler;
    }

    // ==================================================
    // MÉTHODE 1 : GET tous les stagiaires (LISTE)
    // ==================================================
    
    // [HttpGet] → cette méthode répond aux requêtes HTTP GET
    // URL : GET /api/trainee
    [HttpGet]
    
    // IActionResult → interface qui permet de retourner différents résultats HTTP (Ok, NotFound, etc.)
    // [FromQuery] TraineeStatus? status → paramètre optionnel dans l'URL
    //   Exemple : GET /api/trainee?status=InProgress → filtre sur les actifs
    public async Task<IActionResult> GetAll(
        [FromQuery] TraineeStatus? status)
    {
        // 1. Créer la Query avec le filtre (status peut être null)
        var query = new GetTraineesQuery { Status = status };
        
        // 2. Exécuter le Handler qui va chercher les données en base
        var result = await _getHandler.Handle(query);
        
        // 3. Retourner 200 OK avec la liste des TraineeDto
        return Ok(result);
    }

    // ==================================================
    // MÉTHODE 2 : GET un stagiaire par son ID (DÉTAIL)
    // ==================================================
    
    // [HttpGet("{id}")] → paramètre "id" dans l'URL
    // URL : GET /api/trainee/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            // Créer la Query avec l'Id reçu
            var query = new GetTraineeByIdQuery(id);
            
            // Exécuter le Handler
            var result = await _getByIdHandler.Handle(query);
            
            // Retourner 200 OK avec le TraineeDetailDto
            return Ok(result);
        }
        // Si le stagiaire n'existe pas, le Handler lance TraineeNotFoundException
        catch (TraineeNotFoundException ex)
        {
            // Retourner 404 Not Found avec le message d'erreur
            return NotFound(ex.Message);
        }
    }

    // ==================================================
    // MÉTHODE 3 : POST créer un nouveau stagiaire
    // ==================================================
    
    // [HttpPost] → répond aux requêtes HTTP POST
    // URL : POST /api/trainee
    [HttpPost]
    
    // [FromBody] CreateTraineeDto dto → les données sont dans le corps (body) de la requête
    // Le client envoie du JSON qui est automatiquement converti en CreateTraineeDto
    public async Task<IActionResult> Create(
        [FromBody] CreateTraineeDto dto)
    {
        try
        {
            // 1. Créer la Command avec les données reçues
            var command = new CreateTraineeCommand(dto);
            
            // 2. Exécuter le Handler (valide, crée, sauvegarde)
            var result = await _createHandler.Handle(command);
            
            // 3. Retourner 201 Created avec :
            //    - Header Location: /api/trainee/{id}
            //    - Body: le TraineeDto créé
            return CreatedAtAction(
                nameof(GetById),           // Nom de la méthode qui récupère le détail
                new { id = result.Id },    // Paramètre de cette méthode (id = 5)
                result);                   // Le DTO créé
        }
        // Si l'email existe déjà → 409 Conflict
        catch (TraineeAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        // Si une validation échoue (champ vide, date invalide) → 400 Bad Request
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ==================================================
    // MÉTHODE 4 : PUT modifier un stagiaire existant
    // ==================================================
    
    // [HttpPut("{id}")] → paramètre "id" dans l'URL
    // URL : PUT /api/trainee/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,                              // Id dans l'URL
        [FromBody] UpdateTraineeDto dto)     // Nouvelles données dans le body
    {
        try
        {
            // Créer la Command avec l'Id et les nouvelles données
            var command = new UpdateTraineeCommand(id, dto);
            
            // Exécuter le Handler (cherche, modifie, sauvegarde)
            var result = await _updateHandler.Handle(command);
            
            // Retourner 200 OK avec le TraineeDto modifié
            return Ok(result);
        }
        // Si le stagiaire n'existe pas → 404 Not Found
        catch (TraineeNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        // Si le stagiaire est inactif (IsActive = false) → 400 Bad Request
        catch (TraineeNotActiveException ex)
        {
            return BadRequest(ex.Message);
        }
        // Si une validation échoue → 400 Bad Request
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ==================================================
    // MÉTHODE 5 : DELETE supprimer un stagiaire (soft delete)
    // ==================================================
    
    // [HttpDelete("{id}")] → paramètre "id" dans l'URL
    // URL : DELETE /api/trainee/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            // Créer la Command avec l'Id
            var command = new DeleteTraineeCommand(id);
            
            // Exécuter le Handler (soft delete → IsDeleted = true)
            await _deleteHandler.Handle(command);
            
            // Retourner 204 No Content (suppression réussie, pas de données à renvoyer)
            return NoContent();
        }
        // Si le stagiaire n'existe pas → 404 Not Found
        catch (TraineeNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ==================================================
    // MÉTHODE SUPPLEMENTAIRE 6 : GET stagiaires actifs seulement
    // ==================================================
    
    // [HttpGet("active")] → chemin spécifique
    // URL : GET /api/trainee/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        // Créer la Query avec le filtre Status = InProgress
        var query = new GetTraineesQuery
        {
            Status = TraineeStatus.InProgress  // Seulement les stagiaires "En cours"
        };
        
        // Exécuter le Handler
        var result = await _getHandler.Handle(query);
        
        // Retourner 200 OK avec la liste filtrée
        return Ok(result);
    }

    // ==================================================
    // MÉTHODE SUPPLEMENTAIRE 7 : GET détail complet d'un stagiaire
    // ==================================================
    
    // [HttpGet("{id}/details")] → chemin spécifique
    // URL : GET /api/trainee/5/details
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetails(int id)
    {
        try
        {
            // Créer la Query avec l'Id
            var query = new GetTraineeByIdQuery(id);
            
            // Exécuter le Handler (GetWithPhasesAsync → charge aussi les phases)
            var result = await _getByIdHandler.Handle(query);
            
            // Retourner 200 OK avec le détail complet
            return Ok(result);
        }
        catch (TraineeNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

// ======================================================
// RÉSUMÉ DES URLS ET MÉTHODES
// ======================================================

// GET    /api/trainee                    → GetAll() → liste tous
// GET    /api/trainee?status=InProgress  → GetAll() → liste filtrée
// GET    /api/trainee/active             → GetActive() → liste stagiaires actifs
// GET    /api/trainee/5                  → GetById() → détail simple
// GET    /api/trainee/5/details          → GetDetails() → détail avec relations
// POST   /api/trainee                    → Create() → créer
// PUT    /api/trainee/5                  → Update() → modifier
// DELETE /api/trainee/5                  → Delete() → supprimer

// ======================================================
// CODES HTTP RETOURNÉS
// ======================================================

// 200 OK        → succès (liste, détail, modification)
// 201 Created   → création réussie (avec Location header)
// 204 NoContent → suppression réussie
// 400 BadRequest → données invalides
// 404 NotFound   → stagiaire introuvable
// 409 Conflict   → email déjà utilisé

// ======================================================
// FORMULE POUR CRÉER TES PROPRES API
// ======================================================

// Pour créer un nouveau Controller (ex: MentorController) :

// 1. Copie ce fichier
// 2. Remplace "Trainee" par "Mentor"
// 3. Remplace "trainee" par "mentor" dans [Route]
// 4. Adapte les DTOs (CreateMentorDto, UpdateMentorDto, MentorDto)
// 5. Adapte les Handlers (GetMentorHandler, etc.)
// 6. Le reste de la structure reste IDENTIQUE !


*/

