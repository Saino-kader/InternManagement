//"TraineeController est le point d'entrée pour les pages web des stagiaires. Il reçoit les requêtes HTTP, appelle les services pour communiquer avec l'API, et retourne les vues HTML."
// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Importe les DTOs (CreateTraineeDto, UpdateTraineeDto, TraineeDto)
using InterManagement.Application.Features.Trainees.DTOs;

// Importe le service qui appelle l'API (ITraineeApiService)
using InterManagement.Client.Services;

// Importe les entités (TraineeStatus enum)
using InterManagement.Domain.Entities;

// Importe les enums partagés (si besoin)
using InterManagement.Shared.Enums;

// Importe les fonctionnalités MVC (Controller, IActionResult, View, etc.)
using Microsoft.AspNetCore.Mvc;

// ======================================================
// NAMESPACE
// ======================================================
namespace InterManagement.Client.Controllers;

// ======================================================
// CLASSE TRAINEE CONTROLLER
// ======================================================

// public → accessible partout
// class TraineeController → nom du contrôleur
// : BaseController → hérite de BaseController (donc a accès à SetSuccess et SetError)
public class TraineeController : BaseController
{
    // ==================================================
    // CHAMP (VARIABLE DE CLASSE)
    // ==================================================
    
    // private → seulement cette classe peut l'utiliser
    // readonly → assigné une fois dans le constructeur, jamais modifié
    // ITraineeApiService → le service qui appelle l'API
    // _traineeService → nom de la variable
    private readonly ITraineeApiService _traineeService;

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // public → accessible par l'injection de dépendances
    // TraineeController → nom du constructeur
    // ITraineeApiService traineeService → paramètre reçu
    public TraineeController(ITraineeApiService traineeService)
    {
        // Stocke le service reçu dans le champ _traineeService
        _traineeService = traineeService;
    }

    // ==================================================
    // ACTION : INDEX (LISTE DES STAGIAIRES)
    // URL : /Trainee ou /Trainee?status=0
    // ==================================================
    
    // public → accessible via HTTP
    // async Task<IActionResult> → méthode asynchrone qui retourne une vue
    // Index → nom de l'action
    // TraineeStatus? status → paramètre optionnel (nullable) pour filtrer par statut
    public async Task<IActionResult> Index(TraineeStatus? status = null)
    {
        // status.HasValue → est-ce que status n'est pas null ?
        // Si status a une valeur (ex: ?status=0) → appel GetByStatusAsync
        // Si status est null → appel GetAllAsync
        var trainees = status.HasValue
            ? await _traineeService.GetByStatusAsync(status.Value)  // Filtre par statut
            : await _traineeService.GetAllAsync();                         // Tous les stagiaires

        // ViewBag = objet dynamique qui passe des données à la vue
        // ViewBag.CurrentStatus = status → la vue saura quel filtre est actif
        ViewBag.CurrentStatus = status;
        
        // Return View(trainees) → envoie la liste des stagiaires à la vue Index.cshtml
        return View(trainees);
    }

    // ==================================================
    // ACTION : DETAILS (DÉTAIL D'UN STAGIAIRE)
    // URL : /Trainee/Details/5
    // ==================================================
    
    // int id → l'ID du stagiaire vient de l'URL (ex: /Trainee/Details/5)
    public async Task<IActionResult> Details(int id)
    {
        // Appelle l'API pour récupérer le détail du stagiaire (avec ses phases)
        var trainee = await _traineeService.GetDetailsAsync(id);
        
        // Si le stagiaire n'existe pas (API a retourné null)
        if (trainee is null) 
            return NotFound();  // Retourne une page 404
        
        // Retourne la vue Details.cshtml avec le TraineeDetailDto
        return View("~/Views/Stagiaire/Details.cshtml", trainee);
    }

    // ==================================================
    // ACTION : CREATE (AFFICHER LE FORMULAIRE)
    // URL : /Trainee/Create (GET)
    // ==================================================
    
    // Pas de [HttpPost] → cette méthode répond aux requêtes GET
    // IActionResult → retourne une vue
    public IActionResult Create() 
        // => expression body (raccourci)
        // View(new CreateTraineeDto()) → envoie un formulaire vide à la vue Create.cshtml
        => View(new CreateTraineeDto());

    // ==================================================
    // ACTION : CREATE (ENREGISTRER LE FORMULAIRE)
    // URL : /Trainee/Create (POST)
    // ==================================================
    
    // [HttpPost] → cette méthode répond aux requêtes POST (soumission du formulaire)
    // [ValidateAntiForgeryToken] → protection CSRF (empêche les attaques externes)
    // CreateTraineeDto model → ASP.NET remplit automatiquement avec les données du formulaire
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTraineeDto model)
    {
        // Appelle l'API pour créer le stagiaire
        // Si succès → retourne le TraineeDto créé
        // Si échec → retourne null
        var created = await _traineeService.CreateAsync(model);
        
        // Si la création a échoué (email existe déjà, dates invalides, etc.)
        if (created is null)
        {
            // SetError vient de BaseController → stocke un message d'erreur dans TempData
            SetError("Échec de la création du stagiaire. L'email est peut-être déjà utilisé ou les dates sont invalides.");
            // Retourne le formulaire avec les données saisies (pour que l'utilisateur corrige)
            return View(model);
        }

        // SetSuccess vient de BaseController → stocke un message de succès dans TempData
        SetSuccess($"Stagiaire {created.FirstName} {created.LastName} créé avec succès.");
        
        // Redirige vers la liste des stagiaires (Index)
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : EDIT (AFFICHER LE FORMULAIRE DE MODIFICATION)
    // URL : /Trainee/Edit/5 (GET)
    // ==================================================
    
    // int id → l'ID du stagiaire à modifier
    public async Task<IActionResult> Edit(int id)
    {
        // Récupère le stagiaire depuis l'API
        var trainee = await _traineeService.GetByIdAsync(id);
        
        // Si le stagiaire n'existe pas → 404
        if (trainee is null) 
            return NotFound();

        // Convertit TraineeDto en UpdateTraineeDto pour pré-remplir le formulaire
        var dto = new UpdateTraineeDto
        {
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            University = trainee.University,
            Specialty = trainee.Specialty,
            Theme = trainee.Theme,
            StartDate = trainee.StartDate,
            EndDate = trainee.EndDate,
            Status = trainee.Status
        };

        // Passe l'ID à la vue (via ViewBag) pour le formulaire POST
        ViewBag.TraineeId = id;
        
        // Retourne la vue Edit.cshtml avec le DTO pré-rempli
        return View(dto);
    }

    // ==================================================
    // ACTION : EDIT (ENREGISTRER LES MODIFICATIONS)
    // URL : /Trainee/Edit/5 (POST)
    // ==================================================
    
    // [HttpPost] → répond aux requêtes POST
    // int id → l'ID du stagiaire (vient de l'URL)
    // UpdateTraineeDto model → les nouvelles données du formulaire
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateTraineeDto model)
    {
        // Appelle l'API pour modifier le stagiaire
        var updated = await _traineeService.UpdateAsync(id, model);
        
        // Si la modification a échoué
        if (updated is null)
        {
            SetError("Échec de la modification du stagiaire. Vérifiez les données et réessayez.");
            ViewBag.TraineeId = id;  // Passe l'ID à la vue
            return View(model);       // Retourne le formulaire avec les données saisies
        }

        SetSuccess($"Stagiaire {updated.FirstName} {updated.LastName} modifié avec succès.");
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : DELETE (AFFICHER LA CONFIRMATION)
    // URL : /Trainee/Delete/5 (GET)
    // ==================================================
    
    public async Task<IActionResult> Delete(int id)
    {
        var trainee = await _traineeService.GetByIdAsync(id);
        if (trainee is null) return NotFound();
        return View(trainee);  // Affiche la vue Delete.cshtml avec les infos du stagiaire
    }

    // ==================================================
    // ACTION : DELETE (CONFIRMER LA SUPPRESSION)
    // URL : /Trainee/Delete/5 (POST)
    // ==================================================
    
    // [HttpPost, ActionName("Delete")] → cette méthode répond au POST sur l'URL Delete
    // Le nom C# est DeleteConfirmed mais l'URL reste /Trainee/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Appelle l'API pour supprimer le stagiaire
        var success = await _traineeService.DeleteAsync(id);
        
        if (!success)
        {
            SetError("Échec de la suppression du stagiaire.");
            return RedirectToAction(nameof(Delete), new { id });  // Revient à la page de confirmation
        }
        
        SetSuccess("Stagiaire supprimé avec succès.");
        return RedirectToAction(nameof(Index));
    }
}
