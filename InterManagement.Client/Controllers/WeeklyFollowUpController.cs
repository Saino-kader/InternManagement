using InterManagement.Application.Features.WeeklyFollowUps.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class WeeklyFollowUpController : BaseController
{
    private readonly IWeeklyFollowUpApiService _followUpService;

    public WeeklyFollowUpController(IWeeklyFollowUpApiService followUpService)
    {
        _followUpService = followUpService;
    }

    public async Task<IActionResult> Index(int? weekId = null, int? mentorId = null)
    {
        List<WeeklyFollowUpDto> followUps;

        if (weekId.HasValue)
            followUps = await _followUpService.GetByWeekAsync(weekId.Value);
        else if (mentorId.HasValue)
            followUps = await _followUpService.GetByMentorAsync(mentorId.Value);
        else
            followUps = await _followUpService.GetAllAsync();

        return View(followUps);
    }

    public async Task<IActionResult> Details(int id)
    {
        var followUp = await _followUpService.GetByIdAsync(id);
        if (followUp is null) return NotFound();
        return View(followUp);
    }

    public IActionResult Create() => View(new CreateWeeklyFollowUpDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWeeklyFollowUpDto model)
    {
        var created = await _followUpService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création du suivi. Il existe peut-être déjà pour cette semaine.");
            return View(model);
        }

        SetSuccess($"Suivi de la semaine {created.WeekId} créé avec succès.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, string comment)
    {
        var success = await _followUpService.CompleteAsync(id, comment);
        if (!success)
        {
            SetError("Échec du marquage du suivi comme validé.");
        }
        else
        {
            SetSuccess("Suivi marqué comme validé.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkMissed(int id)
    {
        var success = await _followUpService.MarkMissedAsync(id);
        if (!success)
        {
            SetError("Échec du marquage du suivi comme manqué.");
        }
        else
        {
            SetSuccess("Suivi marqué comme manqué.");
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var followUp = await _followUpService.GetByIdAsync(id);
        if (followUp is null) return NotFound();
        return View(followUp);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _followUpService.DeleteAsync(id);
        if (!success)
        {
            SetError("Échec de la suppression du suivi.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Suivi supprimé avec succès.");
        return RedirectToAction(nameof(Index));
    }

    // GET /WeeklyFollowUp/Import
    public IActionResult Import()
    {
        return View();
    }

    // POST /WeeklyFollowUp/Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            SetError("Veuillez sélectionner un fichier.");
            return View();
        }

        using var stream = file.OpenReadStream();
        var result = await _followUpService.ImportAsync(stream, file.FileName);

        if (result == null)
        {
            SetError("L'import a échoué.");
            return View();
        }

        SetSuccess($"{result.SuccessCount}/{result.TotalRowsRead} lignes importées.");

        if (result.Errors.Count > 0)
        {
            foreach (var error in result.Errors)
                SetError(error.Reason);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /WeeklyFollowUp/Export?traineeId=5
    public async Task<IActionResult> Export(int traineeId)
    {
        var result = await _followUpService.ExportByTraineeAsync(traineeId);
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
}




