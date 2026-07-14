// Controllers/WeekController.cs
// Gère l'ajout d'une semaine à une phase existante
// et la suppression d'une semaine.

using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class WeekController : BaseController
{
    private readonly IWeekApiService _weekService;

    public WeekController(IWeekApiService weekService)
    {
        _weekService = weekService;
    }

    // ── Ajouter une semaine à une phase ──────────────────────────────
    // Appelé depuis le modal "Ajouter une semaine" dans _PhaseAccordion
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeekDto model)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        if (model.PhaseId <= 0)
        {
            SetError("Phase introuvable.");
            return RedirectToAction("Index", "Phases");
        }

        if (model.WeekNumber <= 0)
        {
            SetError("Le numéro de semaine doit être supérieur à 0.");
            return RedirectToAction("Index", "Phases");
        }

        var created = await _weekService.CreateAsync(model);

        if (created is null)
        {
            SetError($"La semaine {model.WeekNumber} existe déjà pour cette phase, ou une erreur s'est produite.");
        }
        else
        {
            SetSuccess($"Semaine {created.WeekNumber} ajoutée avec succès.");
        }

        return RedirectToAction("Index", "Phases");
    }

    // ── Supprimer une semaine ─────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        var success = await _weekService.DeleteAsync(id);

        if (!success)
            SetError("Échec de la suppression de la semaine.");
        else
            SetSuccess("Semaine supprimée avec succès.");

        return RedirectToAction("Index", "Phases");
    }
}
