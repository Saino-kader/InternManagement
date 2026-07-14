// Controllers/StagiaireController.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class StagiaireController : BaseController
{
    private readonly IPhaseApiService _phaseService;
    private readonly IWeekApiService _weekService;
    private readonly ITraineeApiService _traineeService;
    private readonly IFeedbackApiService _feedbackService;

    public StagiaireController(
        IPhaseApiService phaseService,
        IWeekApiService weekService,
        ITraineeApiService traineeService,
        IFeedbackApiService feedbackService)
    {
        _phaseService = phaseService;
        _weekService = weekService;
        _traineeService = traineeService;
        _feedbackService = feedbackService;
    }

    public async Task<IActionResult> Index(int traineeId)
    {
        var check = RequireRole("Trainee");
        if (check != null) return check;

        // Sécurité supplémentaire : le stagiaire connecté
        // ne peut voir QUE SA PROPRE page, pas celle d'un autre
        if (CurrentEntityId != traineeId)
            return RedirectToAction("Index",
                new { traineeId = CurrentEntityId });

        var trainee = await _traineeService.GetByIdAsync(traineeId);
        if (trainee is null) return NotFound();

        var phases = await _phaseService.GetByTraineeAsync(traineeId);
        var orderedPhases = phases.OrderBy(p => p.PhaseNumber).ToList();

        // Lance les appels "semaines par phase" en parallèle plutôt
        // qu'en série (une phase n'attend pas la précédente).
        var weeksByPhaseTasks = orderedPhases
            .Select(phase => _weekService.GetByPhaseAsync(phase.Id))
            .ToList();
        await Task.WhenAll(weeksByPhaseTasks);

        var phaseItems = orderedPhases
            .Select((phase, index) => new StagiairePhaseItem
            {
                PhaseId = phase.Id,
                PhaseNumber = phase.PhaseNumber,
                Title = phase.Title,
                MentorId = phase.MentorId,
                MentorName = phase.MentorName,
                Weeks = weeksByPhaseTasks[index].Result
            })
            .ToList();

        var model = new StagiaireViewModel
        {
            TraineeId = traineeId,
            TraineeName = $"{trainee.FirstName} {trainee.LastName}",
            Phases = phaseItems
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendFeedback(CreateFeedbackDto model)
    {
        if (!model.TraineeId.HasValue)
        {
            SetError("TraineeId est obligatoire.");
            return RedirectToAction(nameof(Index), new { traineeId = model.TraineeId });
        }

        var phases = await _phaseService.GetByTraineeAsync(model.TraineeId.Value);
        var mentorId = phases
            .OrderByDescending(p => p.PhaseNumber)
            .FirstOrDefault(p => p.MentorId.HasValue)
            ?.MentorId;

        if (!mentorId.HasValue)
        {
            SetError("Aucun mentor n'est associé à ce stagiaire.");
            return RedirectToAction(nameof(Index), new { traineeId = model.TraineeId.Value });
        }

        model.MentorId = mentorId.Value;
        model.Message = model.Message?.Trim() ?? string.Empty;

        var created = await _feedbackService.CreateAsync(model);

        if (created is null)
        {
            SetError("Échec de l'envoi du feedback. Veuillez réessayer.");
        }
        else
        {
            SetSuccess("Votre feedback a été envoyé avec succès.");
        }

        return RedirectToAction(nameof(Index), new { traineeId = model.TraineeId.Value });
    }
}
