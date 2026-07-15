// Controllers/PhaseController.cs
using InterManagement.Application.Features.Phases.Commands.CreatePhaseForMultipleTrainees;
using InterManagement.Application.Features.Phases.DTOs;
using InterManagement.Application.Features.Weeks.DTOs;
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class PhaseController : BaseController
{
    private readonly IPhaseApiService _phaseService;
    private readonly ITraineeApiService _traineeService;
    private readonly IMentorApiService _mentorService;
    private readonly IWeekApiService _weekService;

    public PhaseController(
        IPhaseApiService phaseService,
        ITraineeApiService traineeService,
        IMentorApiService mentorService,
        IWeekApiService weekService)
    {
        _phaseService   = phaseService;
        _traineeService = traineeService;
        _mentorService  = mentorService;
        _weekService    = weekService;
    }

    public async Task<IActionResult> Index()
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        // Récupère tout en parallèle
        var phasesTask     = _phaseService.GetAllAsync();
        var stagiairesTask = _traineeService.GetAllAsync();
        var mentorsTask    = _mentorService.GetAllAsync();

        await Task.WhenAll(phasesTask, stagiairesTask, mentorsTask);

        var phases = await phasesTask;

        // ── Construit les accordéons ─────────────────────────────────
        // Groupe les phases par PhaseNumber. Le nom du stagiaire et les
        // semaines de TOUTES les phases sont récupérés en parallèle
        // (un aller-retour par phase au lieu d'attendre phase par phase).
        var traineeTasks = phases.ToDictionary(
            p => p.Id, p => _traineeService.GetByIdAsync(p.TraineeId));
        var weekTasks = phases.ToDictionary(
            p => p.Id, p => _weekService.GetByPhaseAsync(p.Id));

        await Task.WhenAll(traineeTasks.Values.Concat<Task>(weekTasks.Values));

        var accordionItems = new List<PhaseAccordionItem>();

        var phaseGroups = phases
            .GroupBy(p => p.PhaseNumber)
            .OrderBy(g => g.Key);

        foreach (var group in phaseGroups)
        {
            var rows = new List<PhaseAccordionRowItem>();

            foreach (var phase in group)
            {
                var trainee = traineeTasks[phase.Id].Result;
                var traineeName = trainee != null
                    ? $"{trainee.FirstName} {trainee.LastName}"
                    : $"Stagiaire #{phase.TraineeId}";

                var weeks = weekTasks[phase.Id].Result;

                rows.Add(new PhaseAccordionRowItem
                {
                    PhaseId     = phase.Id,
                    TraineeName = traineeName,
                    MentorName  = phase.MentorName,
                    Status      = phase.Status.ToString(),
                    Weeks       = weeks.OrderBy(w => w.WeekNumber).ToList()
                });
            }

            // Prend le titre du premier item du groupe
            var firstPhase = group.First();

            accordionItems.Add(new PhaseAccordionItem
            {
                PhaseNumber = group.Key,
                Title       = firstPhase.Title,
                Rows        = rows
            });
        }

        var model = new PhasesViewModel
        {
            Phases         = phases,
            AccordionItems = accordionItems,
            Stagiaires     = await stagiairesTask,
            Mentors        = await mentorsTask
        };

        return View(model);
    }

    // ── Crée une phase pour plusieurs stagiaires ──────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePhaseForMultipleTraineesDto model)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        if (model.TraineeIds == null || model.TraineeIds.Count == 0)
        {
            SetError("Veuillez sélectionner au moins un stagiaire.");
            return RedirectToAction(nameof(Index));
        }

        if (model.Weeks == null || model.Weeks.Count == 0)
        {
            SetError("Veuillez ajouter au moins une semaine.");
            return RedirectToAction(nameof(Index));
        }

        // Vérification des doublons de numéro de semaine côté Client
        var weekNumbers = model.Weeks.Select(w => w.WeekNumber).ToList();
        if (weekNumbers.Distinct().Count() != weekNumbers.Count)
        {
            SetError("Deux semaines ont le même numéro. Corrigez le planning.");
            return RedirectToAction(nameof(Index));
        }

        var created = await _phaseService.CreateForMultipleTraineesAsync(model);

        if (created == null || created.Count == 0)
        {
            SetError("Échec de la création de la phase.");
        }
        else
        {
            SetSuccess($"Phase {model.PhaseNumber} créée pour {created.Count} stagiaire(s).");
        }

        return RedirectToAction(nameof(Index));
    }

    // ── Modifier une phase depuis le modal ─────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditFromModal(PhaseEditModalDto model)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        if (model.PhaseId <= 0 || model.WeekId <= 0)
        {
            SetError("Données de modification invalides.");
            return RedirectToAction(nameof(Index));
        }

        var existingPhase = await _phaseService.GetByIdAsync(model.PhaseId);
        if (existingPhase is null)
        {
            SetError("La phase à modifier est introuvable.");
            return RedirectToAction(nameof(Index));
        }

        var phaseUpdate = new UpdatePhaseDto
        {
            Title = existingPhase.Title,
            StartDate = existingPhase.StartDate,
            EndDate = existingPhase.EndDate,
            Status = model.Status
        };

        var phaseResult = await _phaseService.UpdateAsync(model.PhaseId, phaseUpdate);

        var weekUpdate = new UpdateWeekDto
        {
            Course = model.Course,
            StartDate = model.WeekStartDate,
            EndDate = model.WeekEndDate,
            Status = model.WeekStatus
        };

        var weekResult = await _weekService.UpdateAsync(model.WeekId, weekUpdate);

        if (phaseResult is null || weekResult is null)
        {
            SetError("La modification a échoué. Vérifiez les données puis réessayez.");
            return RedirectToAction(nameof(Index));
        }

        SetSuccess("Phase et semaine modifiées avec succès.");
        return RedirectToAction(nameof(Index));
    }

    // ── Supprimer une phase ─────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var check = RequireRole("Admin");
        if (check != null) return check;

        var success = await _phaseService.DeleteAsync(id);
        if (success)
            SetSuccess("Phase supprimée avec succès.");
        else
            SetError("Échec de la suppression de la phase.");

        return RedirectToAction(nameof(Index));
    }
}