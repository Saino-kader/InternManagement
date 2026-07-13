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







/*
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

    public async Task<IActionResult> Index(int? phaseId = null)
    {
        var weeks = phaseId.HasValue
            ? await _weekService.GetByPhaseAsync(phaseId.Value)
            : await _weekService.GetAllAsync();

        ViewBag.CurrentPhaseId = phaseId;
        return View(weeks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var week = await _weekService.GetByIdAsync(id);
        if (week is null) return NotFound();
        return View(week);
    }

    public IActionResult Create() => View(new CreateWeekDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeekDto model)
    {
        var created = await _weekService.CreateAsync(model);
        if (created is null)
        {
            SetError("Failed to create week. It may already exist for this phase.");
            return View(model);
        }

        SetSuccess($"Week {created.WeekNumber} created successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var week = await _weekService.GetByIdAsync(id);
        if (week is null) return NotFound();

        var dto = new UpdateWeekDto
        {
            Course = week.Course,
            StartDate = week.StartDate,
            EndDate = week.EndDate
        };

        ViewBag.WeekId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateWeekDto model)
    {
        var updated = await _weekService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Failed to update week. Please verify the dates.");
            ViewBag.WeekId = id;
            return View(model);
        }

        SetSuccess($"Week {updated.WeekNumber} updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var week = await _weekService.GetByIdAsync(id);
        if (week is null) return NotFound();
        return View(week);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _weekService.DeleteAsync(id);
        if (!success)
        {
            SetError("Failed to delete week.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Week deleted successfully.");
        return RedirectToAction(nameof(Index));
    }
}


*/



