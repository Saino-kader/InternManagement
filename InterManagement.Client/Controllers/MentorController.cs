using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class MentorController : BaseController
{
    private readonly IMentorApiService _mentorService;

    public MentorController(IMentorApiService mentorService)
    {
        _mentorService = mentorService;
    }



    public async Task<IActionResult> Index(string? department = null)
    {
        var mentors = !string.IsNullOrWhiteSpace(department)
            ? await _mentorService.GetByDepartmentAsync(department)
            : await _mentorService.GetAllAsync();

        ViewBag.CurrentDepartment = department;
        return View(mentors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var mentor = await _mentorService.GetDetailsAsync(id);
        if (mentor is null) return NotFound();
        return View(mentor);
    }

    public IActionResult Create() => View(new CreateMentorDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMentorDto model)
    {
        var created = await _mentorService.CreateAsync(model);
        if (created is null)
        {
            SetError("Failed to create mentor. The email may already be in use.");
            return View(model);
        }

        SetSuccess($"Mentor {created.FirstName} {created.LastName} created successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor is null) return NotFound();

        var dto = new UpdateMentorDto
        {
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Department = mentor.Department,
            Specialty = mentor.Specialty
        };

        ViewBag.MentorId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateMentorDto model)
    {
        var updated = await _mentorService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Failed to update mentor. Please verify the data and try again.");
            ViewBag.MentorId = id;
            return View(model);
        }

        SetSuccess($"Mentor {updated.FirstName} {updated.LastName} updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor is null) return NotFound();
        return View(mentor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _mentorService.DeleteAsync(id);
        if (!success)
        {
            SetError("Failed to delete mentor.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Mentor deleted successfully.");
        return RedirectToAction(nameof(Index));
    }
}


















/*


// ======================================================
// IMPORTATIONS (USING)
// ======================================================

// Importe les DTOs des mentors
// MentorDto : ce qu'on reçoit de l'API
// CreateMentorDto : ce qu'on envoie pour créer
// UpdateMentorDto : ce qu'on envoie pour modifier
using InterManagement.Application.Features.Mentors.DTOs;

// Importe le service qui appelle l'API des mentors
using InterManagement.Client.Services;

// Importe les fonctionnalités MVC
using Microsoft.AspNetCore.Mvc;

// ======================================================
// NAMESPACE
// ======================================================

// Déclare l'espace de noms (dossier) où se trouve ce fichier
namespace InterManagement.Client.Controllers;

// ======================================================
// CLASSE CONTROLLER POUR LA PAGE MENTOR
// ======================================================

// public → accessible partout
// class MentorController → nom du contrôleur
// : BaseController → hérite de BaseController
//   → Donne accès à : SetSuccess(), SetError(), TempData
public class MentorController : BaseController
{
    // ==================================================
    // CHAMP (VARIABLE DE CLASSE)
    // ==================================================
    
    // private → uniquement accessible dans cette classe
    // readonly → une fois assigné, ne peut plus être modifié
    // IMentorApiService → le service qui appelle l'API des mentors
    private readonly IMentorApiService _mentorService;

    // ==================================================
    // CONSTRUCTEUR
    // ==================================================
    
    // public → accessible par l'injection de dépendances
    // MentorController → nom du constructeur
    // IMentorApiService mentorService → paramètre injecté
    public MentorController(IMentorApiService mentorService)
    {
        // Stocke le service reçu dans le champ de la classe
        _mentorService = mentorService;
    }

    // ==================================================
    // ACTION : INDEX — AFFICHE LA LISTE DES MENTORS
    // ==================================================
    
    // public → accessible via HTTP
    // async Task<IActionResult> → méthode asynchrone qui retourne une vue
    // Index → nom de l'action (URL : /Mentor ou /Mentor/Index)
    // string? department = null → paramètre optionnel pour filtrer par département
    // Exemple : /Mentor?department=Engineering
    public async Task<IActionResult> Index(string? department = null)
    {
        // ==============================================
        // ÉTAPE 1 : RÉCUPÉRER LES MENTORS (AVEC OU SANS FILTRE)
        // ==============================================
        
        // !string.IsNullOrWhiteSpace(department) → vérifie si department n'est pas null ou vide
        // Si department est fourni → appelle GetByDepartmentAsync (filtre)
        // Si department est null ou vide → appelle GetAllAsync (tous)
        var mentors = !string.IsNullOrWhiteSpace(department)
            ? await _mentorService.GetByDepartmentAsync(department)
            : await _mentorService.GetAllAsync();

        // ==============================================
        // ÉTAPE 2 : PASSER LE FILTRE À LA VUE
        // ==============================================
        
        // ViewBag.CurrentDepartment = department
        // → Passe le département actuel à la vue
        // → La vue peut afficher : "Filtre actif : Engineering"
        ViewBag.CurrentDepartment = department;

        // ==============================================
        // ÉTAPE 3 : RETOURNER LA VUE
        // ==============================================
        
        // return View("Mentor_Index", mentors)
        // → "Mentor_Index" = nom du fichier de vue
        // → mentors = la liste des mentors à afficher
        // 
        // POURQUOI "Mentor_Index" ?
        // Parce que le fichier s'appelle Mentor_Index.cshtml, pas Index.cshtml
        return View("Mentor_Index", mentors);
    }

    // ==================================================
    // ACTION : DETAILS — AFFICHE LE DÉTAIL D'UN MENTOR
    // ==================================================
    
    // int id → l'ID du mentor (vient de l'URL : /Mentor/Details/5)
    public async Task<IActionResult> Details(int id)
    {
        // ==============================================
        // ÉTAPE 1 : RÉCUPÉRER LE MENTOR
        // ==============================================
        
        // _mentorService.GetDetailsAsync(id)
        // → envoie GET /api/mentor/{id}/details
        // → Retourne MentorDetailDto ou null
        var mentor = await _mentorService.GetDetailsAsync(id);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LE MENTOR EXISTE
        // ==============================================
        
        // if (mentor is null) → si le mentor n'existe pas (API retourne 404)
        if (mentor is null) 
            return NotFound();  // Retourne une page 404
        
        // ==============================================
        // ÉTAPE 3 : RETOURNER LA VUE
        // ==============================================
        
        // return View(mentor) → envoie le mentor à la vue
        // La vue se trouve dans : Views/Mentor/Details.cshtml
        return View(mentor);
    }

    // ==================================================
    // ACTION : CREATE (GET) — AFFICHER LE FORMULAIRE
    // ==================================================
    
    // IActionResult Create() → méthode qui retourne une vue
    // → View(new CreateMentorDto()) → envoie un DTO vide à la vue
    // → La vue génère un formulaire vide
    public IActionResult Create() => View(new CreateMentorDto());

    // ==================================================
    // ACTION : CREATE (POST) — ENREGISTRER LE FORMULAIRE
    // ==================================================
    
    // [HttpPost] → répond aux requêtes POST
    // [ValidateAntiForgeryToken] → protection CSRF
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMentorDto model)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR CRÉER
        // ==============================================
        
        // _mentorService.CreateAsync(model)
        // → envoie POST /api/mentor avec les données
        // → Retourne MentorDto ou null
        var created = await _mentorService.CreateAsync(model);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LA CRÉATION A RÉUSSI
        // ==============================================
        
        if (created is null)
        {
            // SetError → message d'erreur
            SetError("Failed to create mentor. The email may already be in use.");
            
            // return View(model) → retourne le formulaire avec les données saisies
            // L'utilisateur voit ses données et l'erreur
            return View(model);
        }

        // ==============================================
        // ÉTAPE 3 : AFFICHER LE SUCCÈS ET REDIRIGER
        // ==============================================
        
        // SetSuccess → message de succès avec le nom du mentor créé
        SetSuccess($"Mentor {created.FirstName} {created.LastName} created successfully.");
        
        // RedirectToAction(nameof(Index)) → redirige vers /Mentor
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : EDIT (GET) — AFFICHER LE FORMULAIRE DE MODIFICATION
    // ==================================================
    
    // int id → l'ID du mentor (vient de l'URL : /Mentor/Edit/5)
    public async Task<IActionResult> Edit(int id)
    {
        // ==============================================
        // ÉTAPE 1 : RÉCUPÉRER LE MENTOR
        // ==============================================
        
        // _mentorService.GetByIdAsync(id)
        // → envoie GET /api/mentor/{id}
        // → Retourne MentorDto ou null
        var mentor = await _mentorService.GetByIdAsync(id);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LE MENTOR EXISTE
        // ==============================================
        
        if (mentor is null) 
            return NotFound();

        // ==============================================
        // ÉTAPE 3 : CRÉER LE DTO DE MODIFICATION
        // ==============================================
        
        // Convertit MentorDto → UpdateMentorDto
        // Pour pré-remplir le formulaire avec les données actuelles
        var dto = new UpdateMentorDto
        {
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Department = mentor.Department,
            Specialty = mentor.Specialty
        };

        // ==============================================
        // ÉTAPE 4 : PASSER L'ID À LA VUE
        // ==============================================
        
        // ViewBag.MentorId = id → permet de construire l'URL POST
        // Le formulaire saura qu'il modifie le mentor ID = id
        ViewBag.MentorId = id;

        // ==============================================
        // ÉTAPE 5 : RETOURNER LA VUE AVEC LE DTO PRÉ-REMPLI
        // ==============================================
        
        // return View(dto) → envoie le DTO à la vue Edit.cshtml
        return View(dto);
    }

    // ==================================================
    // ACTION : EDIT (POST) — ENREGISTRER LES MODIFICATIONS
    // ==================================================
    
    // [HttpPost] → répond aux requêtes POST
    // int id → l'ID du mentor (vient de l'URL ou du formulaire)
    // UpdateMentorDto model → les nouvelles données
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateMentorDto model)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR MODIFIER
        // ==============================================
        
        // _mentorService.UpdateAsync(id, model)
        // → envoie PUT /api/mentor/{id} avec les nouvelles données
        var updated = await _mentorService.UpdateAsync(id, model);
        
        // ==============================================
        // ÉTAPE 2 : VÉRIFIER SI LA MODIFICATION A RÉUSSI
        // ==============================================
        
        if (updated is null)
        {
            SetError("Failed to update mentor. Please verify the data and try again.");
            ViewBag.MentorId = id;  // Garde l'ID pour le formulaire
            return View(model);     // Retourne le formulaire avec les données saisies
        }

        // ==============================================
        // ÉTAPE 3 : AFFICHER LE SUCCÈS ET REDIRIGER
        // ==============================================
        
        SetSuccess($"Mentor {updated.FirstName} {updated.LastName} updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    // ==================================================
    // ACTION : DELETE (GET) — AFFICHER LA CONFIRMATION
    // ==================================================
    
    // int id → l'ID du mentor à supprimer
    public async Task<IActionResult> Delete(int id)
    {
        // Récupère le mentor pour afficher ses infos
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor is null) return NotFound();
        
        // return View(mentor) → affiche la page de confirmation
        return View(mentor);
    }

    // ==================================================
    // ACTION : DELETE CONFIRMED — SUPPRIMER
    // ==================================================
    
    // [HttpPost, ActionName("Delete")] → répond au POST sur /Mentor/Delete/5
    // → La méthode s'appelle DeleteConfirmed en C# mais l'URL reste /Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // ==============================================
        // ÉTAPE 1 : APPELER L'API POUR SUPPRIMER
        // ==============================================
        
        // _mentorService.DeleteAsync(id)
        // → envoie DELETE /api/mentor/{id}
        // → Retourne true si succès, false si erreur
        var success = await _mentorService.DeleteAsync(id);
        
        // ==============================================
        // ÉTAPE 2 : GÉRER LE RÉSULTAT
        // ==============================================
        
        if (!success)
        {
            SetError("Failed to delete mentor.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Mentor deleted successfully.");
        return RedirectToAction(nameof(Index));
    }
}

// ======================================================
// RÉSUMÉ DES ACTIONS
// ======================================================

// URL                               Méthode        Rôle
// ──────────────────────────────────────────────────────────────
// GET    /Mentor                    Index()        Liste des mentors (avec filtre optionnel)
// GET    /Mentor/Details/5          Details()      Détail d'un mentor
// GET    /Mentor/Create             Create()       Formulaire de création
// POST   /Mentor/Create             Create()       Enregistre le nouveau mentor
// GET    /Mentor/Edit/5             Edit()         Formulaire de modification
// POST   /Mentor/Edit/5             Edit()         Enregistre la modification
// GET    /Mentor/Delete/5           Delete()       Page de confirmation
// POST   /Mentor/Delete/5           DeleteConfirmed() Supprime le mentor

// ======================================================
// CE QUE CE CONTROLLER FAIT
// ======================================================

// 1. Index() → Liste des mentors (filtre par département possible)
// 2. Details() → Détail d'un mentor
// 3. Create() → Ajouter un mentor (GET + POST)
// 4. Edit() → Modifier un mentor (GET + POST)
// 5. Delete() → Supprimer un mentor (GET + POST)

// ======================================================
// POINTS IMPORTANTS
// ======================================================

// 1. return View("Mentor_Index", mentors)
//    → Spécifie le nom exact du fichier de vue
// 
// 2. string? department = null
//    → Paramètre optionnel pour filtrer par département
// 
// 3. ViewBag.CurrentDepartment
//    → Passe le filtre actif à la vue
// 
// 4. ActionName("Delete")
//    → Permet à DeleteConfirmed de répondre à /Mentor/Delete
// 
// 5. RedirectToAction(nameof(Index))
//    → Redirige vers Index après chaque opération

*/
