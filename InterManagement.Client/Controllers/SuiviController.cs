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
        var importesTask = _importedService.GetAllAsync();

        await Task.WhenAll(suivisTask, stagiairesTask, mentorsTask, weeksTask, importesTask);

        var model = new SuiviViewModel
        {
            Suivis = await suivisTask,
            Stagiaires = await stagiairesTask,
            Mentors = await mentorsTask,
            Weeks = await weeksTask,
            SuivisImportes = await importesTask
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

    // GET car déclenché par un lien <a> de téléchargement, pas un formulaire
    public async Task<IActionResult> Export(int traineeId)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

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
        else if (result.SuccessCount == 0)
        {
            var reason = result.Errors.FirstOrDefault() ?? "Vérifiez le format du fichier.";
            SetError($"Aucun suivi n'a pu être importé. {reason}");
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
