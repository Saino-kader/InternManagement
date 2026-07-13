// Controllers/SuiviController.cs
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Application.Features.ImportedFollowUps.DTOs;
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class SuiviController : BaseController
{
    private readonly IWeeklyFollowUpApiService _suiviService;
    private readonly ITraineeApiService _traineeService;
    private readonly IMentorApiService _mentorService;
    private readonly IWeekApiService _weekService;
    private readonly IImportedFollowUpApiService _importedService;

    public SuiviController(
        IWeeklyFollowUpApiService suiviService,
        ITraineeApiService traineeService,
        IMentorApiService mentorService,
        IWeekApiService weekService,
        IImportedFollowUpApiService importedService)
    {
        _suiviService = suiviService;
        _traineeService = traineeService;
        _mentorService = mentorService;
        _weekService = weekService;
        _importedService = importedService;
    }

    public async Task<IActionResult> Index()
    {

        var check = RequireRole("Admin");
        if (check != null) return check;

        var suivisTask = _suiviService.GetAllAsync();
        var stagiairesTask = _traineeService.GetAllAsync();
        var mentorsTask = _mentorService.GetAllAsync();
        var weeksTask = _weekService.GetAllAsync();

        await Task.WhenAll(suivisTask, stagiairesTask, mentorsTask, weeksTask);

        var model = new SuiviViewModel
        {
            Suivis = await suivisTask,
            Stagiaires = await stagiairesTask,
            Mentors = await mentorsTask,
            Weeks = await weeksTask
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeeklyFollowUpDto model)
    {
        var created = await _suiviService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création du suivi. Il existe peut-être déjà pour cette semaine.");
        }
        else
        {
            SetSuccess($"Suivi de la semaine {created.WeekId} créé avec succès.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateWeeklyFollowUpDto model)
    {
        var updated = await _suiviService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Échec de la modification du suivi.");
        }
        else
        {
            SetSuccess("Suivi modifié avec succès.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            SetError("Veuillez sélectionner un fichier.");
            return RedirectToAction(nameof(Index));
        }

        using var stream = file.OpenReadStream();
        var result = await _suiviService.ImportAsync(stream, file.FileName);

        if (result == null)
        {
            SetError("L'import a échoué. Vérifiez le format du fichier.");
        }
        else
        {
            SetSuccess($"{result.SuccessCount} suivi(s) importé(s). {result.ErrorCount} erreur(s).");
        }

        return RedirectToAction(nameof(Index));
    }

    // GET car déclenché par un lien <a> de téléchargement, pas un formulaire
    public async Task<IActionResult> Export(int traineeId)
    {
        var result = await _suiviService.ExportByTraineeAsync(traineeId);

        if (result == null)
        {
            SetError("L'export a échoué pour ce stagiaire.");
            return RedirectToAction(nameof(Index));
        }

        return File(
            result.Value.fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.fileName);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _suiviService.DeleteAsync(id);
        if (success)
            SetSuccess("Suivi supprimé avec succès.");
        else
            SetError("Échec de la suppression du suivi.");

        return RedirectToAction(nameof(Index));
    }




    // ── Suivis importés depuis Excel ─────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportImported(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            SetError("Veuillez sélectionner un fichier.");
            return RedirectToAction(nameof(Index));
        }

        using var stream = file.OpenReadStream();
        var result = await _importedService.ImportAsync(stream, file.FileName);

        if (result == null)
        {
            SetError("L'import a échoué. Vérifiez le format du fichier.");
        }
        else
        {
            SetSuccess($"{result.SuccessCount} suivi(s) importé(s) avec succès. " +
                      $"{result.ErrorCount} erreur(s).");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditImported(
        int id, UpdateImportedFollowUpDto model)
    {
        var updated = await _importedService.UpdateAsync(id, model);
        if (updated is null)
            SetError("Échec de la modification.");
        else
            SetSuccess("Suivi importé modifié.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImported(int id)
    {
        var success = await _importedService.DeleteAsync(id);
        if (!success)
            SetError("Échec de la suppression.");
        else
            SetSuccess("Suivi importé supprimé.");

        return RedirectToAction(nameof(Index));
    }


}







/*

```csharp
// Controllers/SuiviController.cs

// ======================= USING =======================

// DTO (Data Transfer Object) utilisés pour créer et modifier un suivi.
// Les DTO servent uniquement à transporter des données entre le Client et l'API.
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

// ViewModel utilisé pour envoyer plusieurs listes à la vue (Suivis, Stagiaires, Mentors, Weeks).
using InterManagement.Client.Models;

// Services qui communiquent avec l'API via HttpClient.
// Le contrôleur ne dialogue jamais directement avec la base de données.
using InterManagement.Client.Services;

using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

// =======================================================================
// SuiviController hérite de BaseController.
//
// Grâce à cet héritage, ce contrôleur possède automatiquement :
// - la vérification de l'authentification
// - SetSuccess()
// - SetError()
// - CurrentUserRole
// - CurrentUserEmail
// - CurrentEntityId
//
// Le code n'a donc pas besoin d'être réécrit ici.
// =======================================================================
public class SuiviController : BaseController
{
    // ==========================================================
    // DEPENDENCY INJECTION (Injection de dépendances)
    // ==========================================================
    //
    // Le contrôleur ne crée jamais lui-même ses services.
    // ASP.NET Core les fournit automatiquement via le constructeur.
    //
    // Chaque service encapsule les appels à l'API.
    // Le contrôleur ne connaît pas les détails des requêtes HTTP.
    // ==========================================================

    private readonly IWeeklyFollowUpApiService _suiviService;
    private readonly ITraineeApiService _traineeService;
    private readonly IMentorApiService _mentorService;
    private readonly IWeekApiService _weekService;

    // ==========================================================
    // CONSTRUCTEUR
    // ==========================================================
    //
    // Le constructeur est exécuté lorsqu'une instance du contrôleur
    // est créée.
    //
    // ASP.NET Core injecte automatiquement les services enregistrés
    // dans Program.cs.
    // ==========================================================
    public SuiviController(
        IWeeklyFollowUpApiService suiviService,
        ITraineeApiService traineeService,
        IMentorApiService mentorService,
        IWeekApiService weekService)
    {
        _suiviService = suiviService;
        _traineeService = traineeService;
        _mentorService = mentorService;
        _weekService = weekService;
    }

    // ==========================================================
    // ACTION INDEX
    // ==========================================================
    //
    // IActionResult représente le résultat retourné par une action
    // (View, Redirect, Json, File, etc.).
    //
    // async permet d'exécuter du code asynchrone sans bloquer le thread.
    // await attend la fin d'une opération asynchrone.
    // ==========================================================
    public async Task<IActionResult> Index()
    {
        // Chaque appel retourne immédiatement une Task.
        // Les requêtes HTTP démarrent en parallèle.
        var suivisTask = _suiviService.GetAllAsync();
        var stagiairesTask = _traineeService.GetAllAsync();
        var mentorsTask = _mentorService.GetAllAsync();
        var weeksTask = _weekService.GetAllAsync();

        // Attend que toutes les requêtes soient terminées.
        // Plus performant que les attendre une par une.
        await Task.WhenAll(
            suivisTask,
            stagiairesTask,
            mentorsTask,
            weeksTask);

        // Création du ViewModel envoyé à la vue.
        //
        // Le ViewModel regroupe plusieurs données nécessaires
        // pour afficher la page.
        var model = new SuiviViewModel
        {
            Suivis = await suivisTask,
            Stagiaires = await stagiairesTask,
            Mentors = await mentorsTask,
            Weeks = await weeksTask
        };

        // Retourne la vue Index.cshtml
        // en lui transmettant le ViewModel.
        return View(model);
    }

    // ==========================================================
    // CREATE
    // ==========================================================
    //
    // HttpPost :
    // cette action répond uniquement aux requêtes POST.
    //
    // ValidateAntiForgeryToken :
    // protège contre les attaques CSRF.
    // Le formulaire doit contenir :
    // @Html.AntiForgeryToken()
    // ==========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeeklyFollowUpDto model)
    {
        // Envoie les données à l'API.
        var created = await _suiviService.CreateAsync(model);

        // Si null => création échouée.
        if (created is null)
        {
            SetError("Échec de la création du suivi. Il existe peut-être déjà pour cette semaine.");
        }
        else
        {
            SetSuccess($"Suivi de la semaine {created.WeekId} créé avec succès.");
        }

        // PRG Pattern (Post Redirect Get)
        // Evite le renvoi du formulaire lors d'un rafraîchissement.
        return RedirectToAction(nameof(Index));
    }

    // ==========================================================
    // EDIT
    // ==========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateWeeklyFollowUpDto model)
    {
        var updated = await _suiviService.UpdateAsync(id, model);

        if (updated is null)
            SetError("Échec de la modification du suivi.");
        else
            SetSuccess("Suivi modifié avec succès.");

        return RedirectToAction(nameof(Index));
    }

    // ==========================================================
    // IMPORT EXCEL
    // ==========================================================
    //
    // IFormFile représente un fichier envoyé depuis un formulaire HTML.
    // ==========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        // Vérifie que le fichier existe.
        if (file == null || file.Length == 0)
        {
            SetError("Veuillez sélectionner un fichier.");
            return RedirectToAction(nameof(Index));
        }

        // Ouvre le fichier sous forme de Stream.
        //
        // Stream = flux de données permettant de lire
        // le contenu du fichier.
        //
        // using garantit que les ressources seront libérées
        // automatiquement à la fin.
        using var stream = file.OpenReadStream();

        // Envoie le fichier à l'API.
        var result = await _suiviService.ImportAsync(
            stream,
            file.FileName);

        if (result == null)
        {
            SetError("L'import a échoué. Vérifiez le format du fichier.");
        }
        else
        {
            SetSuccess(
                $"{result.SuccessCount} suivi(s) importé(s). {result.ErrorCount} erreur(s).");
        }

        return RedirectToAction(nameof(Index));
    }

    // ==========================================================
    // EXPORT EXCEL
    // ==========================================================
    //
    // Cette action est appelée par un lien <a>.
    //
    // Une requête GET est donc suffisante.
    // ==========================================================
    public async Task<IActionResult> Export(int traineeId)
    {
        var result = await _suiviService.ExportByTraineeAsync(traineeId);

        if (result == null)
        {
            SetError("L'export a échoué pour ce stagiaire.");
            return RedirectToAction(nameof(Index));
        }

        // File() retourne un fichier au navigateur.
        //
        // fileBytes : contenu du fichier Excel.
        // contentType : type MIME du fichier.
        // fileName : nom proposé au téléchargement.
        return File(
            result.Value.fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.fileName);
    }

    // ==========================================================
    // DELETE
    // ==========================================================
    //
    // Supprime un suivi.
    //
    // L'API retourne true si la suppression a réussi,
    // sinon false.
    // ==========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _suiviService.DeleteAsync(id);

        if (success)
            SetSuccess("Suivi supprimé avec succès.");
        else
            SetError("Échec de la suppression du suivi.");

        return RedirectToAction(nameof(Index));
    }
}
```







*/






/*


// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Importe les DTOs des suivis hebdomadaires
// WeeklyFollowUpDto : ce qu'on reçoit de l'API
// CreateWeeklyFollowUpDto : ce qu'on envoie pour créer
// UpdateWeeklyFollowUpDto : ce qu'on envoie pour modifier
using InterManagement.Application.Features.WeeklyFollowUps.DTOs;

// Importe les ViewModels (modèles de données pour les vues)
// SuiviViewModel contient : une liste de suivis, une liste de stagiaires, une liste de mentors
using InterManagement.Client.Models;

// Importe les services qui appellent l'API
// IWeeklyFollowUpApiService : service pour les suivis
// ITraineeApiService : service pour les stagiaires
// IMentorApiService : service pour les mentors
using InterManagement.Client.Services;

// Importe les fonctionnalités MVC
// Controller, IActionResult, View, RedirectToAction, etc.
using Microsoft.AspNetCore.Mvc;

// ======================================================
// NAMESPACE
// ======================================================

// Déclare l'espace de noms (dossier) où se trouve ce fichier
// Correspond au chemin : Controllers/
// Ce fichier est dans le dossier Controllers du projet Client
namespace InterManagement.Client.Controllers;

// ======================================================
// CLASSE CONTROLLER POUR LA PAGE SUIVI
// ======================================================

// public → accessible partout
// class SuiviController → nom du contrôleur
// : BaseController → hérite de BaseController
//   → Donne accès à : SetSuccess(), SetError(), TempData
public class SuiviController : BaseController
{
    // ==================================================
    // CHAMPS (VARIABLES DE CLASSE)
    // ==================================================
    
    // private → uniquement accessible dans cette classe
    // readonly → une fois assigné, ne peut plus être modifié
    // IWeeklyFollowUpApiService → le service qui appelle l'API des suivis
    private readonly IWeeklyFollowUpApiService _suiviService;
    
    // private readonly ITraineeApiService → le service qui appelle l'API des stagiaires
    private readonly ITraineeApiService _traineeService;
    
    // private readonly IMentorApiService → le service qui appelle l'API des mentors
    private readonly IMentorApiService _mentorService;

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // public → accessible par l'injection de dépendances
    // SuiviController → nom du constructeur (identique à la classe)
    // Les paramètres sont injectés automatiquement via Program.cs
    public SuiviController(
        IWeeklyFollowUpApiService suiviService,
        ITraineeApiService traineeService,
        IMentorApiService mentorService)
    {
        // Stocke les services reçus dans les champs de la classe
        _suiviService = suiviService;
        _traineeService = traineeService;
        _mentorService = mentorService;
    }

    // ==================================================
    // ACTION : INDEX — AFFICHE LA PAGE SUIVI
    // ==================================================
    
    // public → accessible via HTTP
    // async Task<IActionResult> → méthode asynchrone qui retourne une vue
    // Index → nom de l'action (URL : /Suivi ou /Suivi/Index)
    public async Task<IActionResult> Index()
    {
        // ==============================================
        // ÉTAPE 1 : LANCER LES 3 APPELS EN PARALLÈLE
        // ==============================================
        
        // Task.WhenAll = exécute plusieurs tâches en même temps
        // Sans WhenAll : les appels seraient séquentiels (100ms + 100ms + 100ms = 300ms)
        // Avec WhenAll : les 3 appels sont en parallèle (100ms maximum)
        
        // Lance l'appel pour récupérer tous les suivis
        // Ne stocke pas le résultat directement, mais la "promesse" de résultat
        var suivisTask = _suiviService.GetAllAsync();
        
        // Lance l'appel pour récupérer tous les stagiaires
        // (nécessaire pour les dropdowns dans le formulaire)
        var stagiairesTask = _traineeService.GetAllAsync();
        
        // Lance l'appel pour récupérer tous les mentors
        // (nécessaire pour les dropdowns dans le formulaire)
        var mentorsTask = _mentorService.GetAllAsync();

        // ==============================================
        // ÉTAPE 2 : ATTENDRE QUE TOUS LES APPELS SOIENT FINIS
        // ==============================================
        
        // Task.WhenAll = attend que les 3 tâches soient terminées
        // Si une tâche échoue, les autres continuent
        await Task.WhenAll(suivisTask, stagiairesTask, mentorsTask);

        // ==============================================
        // ÉTAPE 3 : RÉCUPÉRER LES RÉSULTATS ET CRÉER LE MODÈLE
        // ==============================================
        
        // Crée un nouveau ViewModel (conteneur de données)
        var model = new SuiviViewModel
        {
            // await suivisTask → récupère le résultat de l'appel (List<WeeklyFollowUpDto>)
            Suivis = await suivisTask,
            
            // await stagiairesTask → récupère le résultat (List<TraineeDto>)
            Stagiaires = await stagiairesTask,
            
            // await mentorsTask → récupère le résultat (List<MentorDto>)
            Mentors = await mentorsTask
        };

        // ==============================================
        // ÉTAPE 4 : RETOURNER LA VUE AVEC LE MODÈLE
        // ==============================================
        
        // return View(model) → envoie le modèle à la vue
        // La vue se trouve dans : Views/Suivi/Suivi_Index.cshtml
        // Le modèle est accessible avec @model SuiviViewModel
        // 
        // POURQUOI "Suivi_Index" ?
        // Parce que le fichier s'appelle Suivi_Index.cshtml, pas Index.cshtml
        return View("Suivi_Index", model);
    }

    // ==================================================
    // ACTION : CREATE — AJOUTER UN SUIVI
    // ==================================================
    
    // [HttpPost] → cette méthode répond aux requêtes POST (soumission de formulaire)
    // [ValidateAntiForgeryToken] → protection CSRF (sécurité)
    // CreateWeeklyFollowUpDto model → données du formulaire (ASP.NET les remplit automatiquement)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeeklyFollowUpDto model)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR CRÉER LE SUIVI
        // ==============================================
        
        // _suiviService.CreateAsync(model) → envoie POST /api/weeklyfollowup avec les données
        // Retourne le suivi créé (avec son ID) ou null si erreur
        var created = await _suiviService.CreateAsync(model);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LA CRÉATION A RÉUSSI
        // ==============================================
        
        // if (created is null) → si la création a échoué (API retourne null)
        if (created is null)
        {
            // SetError → vient de BaseController, stocke un message d'erreur dans TempData
            // Le message sera affiché sur la page après redirection
            SetError("Échec de la création du suivi. Il existe peut-être déjà pour cette semaine.");
        }
        else
        {
            // SetSuccess → vient de BaseController, stocke un message de succès dans TempData
            // created.WeekNumber → le numéro de semaine du suivi créé
            SetSuccess($"Suivi de la semaine {created.WeekNumber} créé avec succès.");
        }
        
        // ==============================================
        // ÉTAPE 3 : REDIRIGER VERS LA PAGE SUIVI
        // ==============================================
        
        // RedirectToAction(nameof(Index)) → redirige vers /Suivi (l'action Index)
        // Les messages TempData seront affichés dans la page
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : EDIT — MODIFIER UN SUIVI
    // ==================================================
    
    // [HttpPost] → répond aux requêtes POST
    // int id → l'ID du suivi à modifier (vient du formulaire)
    // UpdateWeeklyFollowUpDto model → les nouvelles données
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateWeeklyFollowUpDto model)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR MODIFIER LE SUIVI
        // ==============================================
        
        // _suiviService.UpdateAsync(id, model) → envoie PUT /api/weeklyfollowup/{id}
        // Retourne le suivi modifié ou null si erreur
        var updated = await _suiviService.UpdateAsync(id, model);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LA MODIFICATION A RÉUSSI
        // ==============================================
        
        if (updated is null)
        {
            SetError("Échec de la modification du suivi.");
        }
        else
        {
            SetSuccess("Suivi modifié avec succès.");
        }
        
        // ==============================================
        // ÉTAPE 3 : REDIRIGER VERS LA PAGE SUIVI
        // ==============================================
        
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : IMPORT — IMPORTER UN FICHIER EXCEL
    // ==================================================
    
    // [HttpPost] → répond aux requêtes POST
    // IFormFile file → le fichier uploadé (ASP.NET le reçoit automatiquement)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        // ==============================================
        // ÉTAPE 1 : VÉRIFIER QUE LE FICHIER EST PRÉSENT
        // ==============================================
        
        // file == null → aucun fichier sélectionné
        // file.Length == 0 → fichier vide
        if (file == null || file.Length == 0)
        {
            SetError("Veuillez sélectionner un fichier.");
            return RedirectToAction(nameof(Index));
        }

        // ==============================================
        // ÉTAPE 2 : LIRE LE FICHIER ET L'ENVOYER À L'API
        // ==============================================
        
        // file.OpenReadStream() → ouvre le fichier en lecture
        using var stream = file.OpenReadStream();
        
        // _suiviService.ImportAsync(stream, file.FileName)
        // → envoie POST /api/weeklyfollowup/import avec le fichier
        // → Retourne ImportResultDto ou null
        var result = await _suiviService.ImportAsync(stream, file.FileName);

        // ==============================================
        // ÉTAPE 3 : AFFICHER LE RÉSULTAT
        // ==============================================
        
        if (result == null)
        {
            SetError("L'import a échoué. Vérifiez le format du fichier.");
        }
        else
        {
            // result.SuccessCount → nombre de lignes importées
            // result.ErrorCount → nombre de lignes en erreur
            SetSuccess($"{result.SuccessCount} suivi(s) importé(s). {result.ErrorCount} erreur(s).");
        }

        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : EXPORT — TÉLÉCHARGER LES SUIVIS D'UN STAGIAIRE
    // ==================================================
    
    // GET → cette méthode répond aux requêtes GET (lien <a> de téléchargement)
    // int traineeId → l'ID du stagiaire (vient de l'URL : /Suivi/Export/5)
    public async Task<IActionResult> Export(int traineeId)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR EXPORTER
        // ==============================================
        
        // _suiviService.ExportByTraineeAsync(traineeId)
        // → envoie GET /api/weeklyfollowup/export/5
        // → Retourne (byte[] fileBytes, string fileName) ou null
        var result = await _suiviService.ExportByTraineeAsync(traineeId);

        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI L'EXPORT A RÉUSSI
        // ==============================================
        
        if (result == null)
        {
            SetError("L'export a échoué pour ce stagiaire.");
            return RedirectToAction(nameof(Index));
        }

        // ==============================================
        // ÉTAPE 3 : RETOURNER LE FICHIER POUR TÉLÉCHARGEMENT
        // ==============================================
        
        // return File(
        //     bytes,          → les octets du fichier
        //     mimeType,       → le type de fichier (.xlsx)
        //     fileName        → le nom du fichier pour le téléchargement
        // )
        return File(
            result.Value.fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Value.fileName);
    }
}

// ======================================================
// RÉSUMÉ DES ACTIONS
// ======================================================

// URL                               Méthode        Rôle
// ──────────────────────────────────────────────────────────────
// GET    /Suivi                     Index()        Affiche la page
// POST   /Suivi/Create              Create()       Crée un suivi
// POST   /Suivi/Edit/{id}           Edit()         Modifie un suivi
// POST   /Suivi/Import              Import()       Importe un fichier Excel
// GET    /Suivi/Export/{traineeId}  Export()       Télécharge les suivis d'un stagiaire

// ======================================================
// CE QUE CE CONTROLLER FAIT
// ======================================================

// 1. Index() → Affiche la page Suivi avec 3 listes : suivis, stagiaires, mentors
// 2. Create() → Ajoute un nouveau suivi (POST)
// 3. Edit()   → Modifie un suivi existant (POST)
// 4. Import() → Importe des suivis depuis un fichier Excel
// 5. Export() → Télécharge les suivis d'un stagiaire en Excel

// ======================================================
// POINTS IMPORTANTS
// ======================================================

// 1. Task.WhenAll → exécute les 3 appels en parallèle (plus rapide)
// 2. [ValidateAntiForgeryToken] → sécurité CSRF
// 3. SetSuccess / SetError → messages flash
// 4. RedirectToAction → redirige vers Index après chaque action
// 5. File() → retourne un fichier pour téléchargement

*/




