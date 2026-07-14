// InterManagement.Client/Controllers/UtilisateursController.cs
// Crée le compte Identity après chaque ajout d'utilisateur et passe
// le mot de passe temporaire à la vue via TempData.

using InterManagement.Application.Features.Trainees.DTOs;
using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class UtilisateursController : BaseController
{
    private readonly ITraineeApiService _traineeService;
    private readonly IMentorApiService _mentorService;
    private readonly IAdminApiService _adminService;
    private readonly IAuthApiService _authService;

    public UtilisateursController(
        ITraineeApiService traineeService,
        IMentorApiService mentorService,
        IAdminApiService adminService,
        IAuthApiService authService)
    {
        _traineeService = traineeService;
        _mentorService = mentorService;
        _adminService = adminService;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        var stagiairesTask = _traineeService.GetAllAsync();
        var mentorsTask = _mentorService.GetAllAsync();
        var adminsTask = _adminService.GetAllAsync();

        await Task.WhenAll(stagiairesTask, mentorsTask, adminsTask);

        var model = new UtilisateursViewModel
        {
            Stagiaires = await stagiairesTask,
            Mentors = await mentorsTask,
            Admins = await adminsTask
        };

        return View(model);
    }

    // ── CRÉATION STAGIAIRE ───────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStagiaire(CreateTraineeDto model)
    {
        var created = await _traineeService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création du stagiaire.");
            return RedirectToAction(nameof(Index));
        }

        // Crée le compte Identity et récupère le mot de passe temporaire
        var tempPassword = await _authService.CreateAccountAsync(
            model.Email, "Trainee");

        SetSuccess($"Stagiaire {created.FirstName} {created.LastName} créé.");

        if (tempPassword != null)
        {
            // Passe le mot de passe à la vue via TempData
            // Il sera affiché UNE SEULE FOIS dans #passwordModal
            TempData["TempPassword"] = tempPassword;
            TempData["TempPasswordEmail"] = model.Email;
            TempData["TempPasswordRole"] = "Stagiaire";
        }

        return RedirectToAction(nameof(Index));
    }

    // ── CRÉATION MENTOR ──────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMentor(CreateMentorDto model)
    {
        var created = await _mentorService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création du mentor.");
            return RedirectToAction(nameof(Index));
        }

        var tempPassword = await _authService.CreateAccountAsync(
            model.Email, "Mentor");

        SetSuccess($"Mentor {created.FirstName} {created.LastName} créé.");

        if (tempPassword != null)
        {
            TempData["TempPassword"] = tempPassword;
            TempData["TempPasswordEmail"] = model.Email;
            TempData["TempPasswordRole"] = "Mentor";
        }

        return RedirectToAction(nameof(Index));
    }

    // ── CRÉATION ADMIN ───────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdmin(CreateAdminDto model)
    {
        var created = await _adminService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création de l'administrateur.");
            return RedirectToAction(nameof(Index));
        }

        var tempPassword = await _authService.CreateAccountAsync(
            model.Email, "Admin");

        SetSuccess($"Admin {created.FirstName} {created.LastName} créé.");

        if (tempPassword != null)
        {
            TempData["TempPassword"] = tempPassword;
            TempData["TempPasswordEmail"] = model.Email;
            TempData["TempPasswordRole"] = "Admin";
        }

        return RedirectToAction(nameof(Index));
    }

    // ── MODIFICATION STAGIAIRE ───────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStagiaire(int id, UpdateTraineeDto model)
    {
        var updated = await _traineeService.UpdateAsync(id, model);
        if (updated is null)
            SetError("Échec de la modification du stagiaire.");
        else
            SetSuccess($"Stagiaire {updated.FirstName} {updated.LastName} modifié.");

        return RedirectToAction(nameof(Index));
    }

    // ── MODIFICATION MENTOR ──────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMentor(int id, UpdateMentorDto model)
    {
        var updated = await _mentorService.UpdateAsync(id, model);
        if (updated is null)
            SetError("Échec de la modification du mentor.");
        else
            SetSuccess($"Mentor {updated.FirstName} {updated.LastName} modifié.");

        return RedirectToAction(nameof(Index));
    }

    // ── MODIFICATION ADMIN ───────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAdmin(int id, UpdateAdminDto model)
    {
        var updated = await _adminService.UpdateAsync(id, model);
        if (updated is null)
            SetError("Échec de la modification de l'administrateur.");
        else
            SetSuccess($"Admin {updated.FirstName} {updated.LastName} modifié.");

        return RedirectToAction(nameof(Index));
    }


    // ── SUPPRESSION ───────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStagiaire(int id)
    {
        // Récupère l'email avant suppression (pour supprimer aussi
        // le compte Identity correspondant)
        var trainee = await _traineeService.GetByIdAsync(id);
        if (trainee != null)
        {
            var deleted = await _traineeService.DeleteAsync(id);
            if (!deleted)
            {
                SetError("Échec de la suppression du stagiaire.");
                return RedirectToAction(nameof(Index));
            }
            await _authService.DeleteAccountAsync(trainee.Email);
            SetSuccess($"{trainee.FirstName} {trainee.LastName} supprimé.");
        }
        else
        {
            SetError("Stagiaire introuvable.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMentor(int id)
    {
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor != null)
        {
            var deleted = await _mentorService.DeleteAsync(id);
            if (!deleted)
            {
                SetError("Échec de la suppression du mentor.");
                return RedirectToAction(nameof(Index));
            }
            await _authService.DeleteAccountAsync(mentor.Email);
            SetSuccess($"{mentor.FirstName} {mentor.LastName} supprimé.");
        }
        else
        {
            SetError("Mentor introuvable.");
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAdmin(int id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        if (admin != null)
        {
            var deleted = await _adminService.DeleteAsync(id);
            if (!deleted)
            {
                SetError("Échec de la suppression de l'administrateur.");
                return RedirectToAction(nameof(Index));
            }
            await _authService.DeleteAccountAsync(admin.Email);
            SetSuccess($"{admin.FirstName} {admin.LastName} supprimé.");
        }
        else
        {
            SetError("Administrateur introuvable.");
        }
        return RedirectToAction(nameof(Index));
    }
}
