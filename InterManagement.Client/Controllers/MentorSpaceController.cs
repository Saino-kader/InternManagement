// Controllers/MentorSpaceController.cs
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class MentorSpaceController : BaseController
{
    private readonly IMentorApiService _mentorService;
    private readonly IAssignmentApiService _assignmentService;
    private readonly IPhaseApiService _phaseService;
    private readonly IWeekApiService _weekService;
    private readonly ITraineeApiService _traineeService;
    private readonly IFeedbackApiService _feedbackService;

    public MentorSpaceController(
        IMentorApiService mentorService,
        IAssignmentApiService assignmentService,
        IPhaseApiService phaseService,
        IWeekApiService weekService,
        ITraineeApiService traineeService,
        IFeedbackApiService feedbackService)
    {
        _mentorService     = mentorService;
        _assignmentService = assignmentService;
        _phaseService      = phaseService;
        _weekService       = weekService;
        _traineeService    = traineeService;
        _feedbackService   = feedbackService;
    }

    public async Task<IActionResult> Index(int mentorId)
    {
        var check = RequireRole("Mentor");
        if (check != null) return check;

        if (CurrentEntityId != mentorId)
            return RedirectToAction("Index", new { mentorId = CurrentEntityId });

        var mentor = await _mentorService.GetByIdAsync(mentorId);
        if (mentor is null) return NotFound();

        var assignedRowsTask   = BuildAssignedTraineesAsync(mentorId);
        var feedbacksMentorTask = _feedbackService.GetByMentorAsync(mentorId);
        var stagiairesTask      = _traineeService.GetAllAsync();

        await Task.WhenAll(assignedRowsTask, feedbacksMentorTask, stagiairesTask);

        var assignedRows       = await assignedRowsTask;
        var feedbacksMentor    = await feedbacksMentorTask;
        var stagiaires         = await stagiairesTask;

        ViewBag.MentorId     = mentorId;
        ViewBag.TraineeCount = assignedRows
            .Select(r => r.TraineeId)
            .Distinct()
            .Count();

        var model = new MentorViewModel
        {
            MentorId            = mentorId,
            MentorName          = $"{mentor.FirstName} {mentor.LastName}",
            StagiairesAssignes  = assignedRows,
            Stagiaires          = stagiaires,
            HistoriqueFeedbacks = feedbacksMentor
        };

        return View("~/Views/Mentor/Index.cshtml", model);
    }

    // ── Construction tableau stagiaires ──────────────────────────────
    private async Task<List<MentorAssignedTraineeItem>> BuildAssignedTraineesAsync(
        int mentorId)
    {
        var rows = new List<MentorAssignedTraineeItem>();
        try
        {
            var assignments = await _assignmentService.GetByMentorAsync(mentorId);
            if (assignments == null || !assignments.Any()) return rows;

            foreach (var assignment in assignments.Where(a => a.IsActive))
            {
                var trainee = await _traineeService.GetByIdAsync(assignment.TraineeId);
                if (trainee == null) continue;

                var phase = await _phaseService.GetByIdAsync(assignment.PhaseId);
                if (phase == null) continue;

                var weeks = await _weekService.GetByPhaseAsync(assignment.PhaseId);

                if (weeks == null || !weeks.Any())
                {
                    rows.Add(new MentorAssignedTraineeItem
                    {
                        TraineeId     = assignment.TraineeId,
                        TraineeName   = $"{trainee.FirstName} {trainee.LastName}",
                        PhaseTitle    = phase.Title,
                        PhaseNumber   = phase.PhaseNumber.ToString(),
                        WeekNumber    = 0,
                        CourseName    = "—",
                        WeekStartDate = null,
                        WeekEndDate   = null,
                        StatusText    = phase.Status.ToString()
                    });
                }
                else
                {
                    foreach (var week in weeks.OrderBy(w => w.WeekNumber))
                    {
                        rows.Add(new MentorAssignedTraineeItem
                        {
                            TraineeId     = assignment.TraineeId,
                            TraineeName   = $"{trainee.FirstName} {trainee.LastName}",
                            PhaseTitle    = phase.Title,
                            PhaseNumber   = phase.PhaseNumber.ToString(),
                            WeekNumber    = week.WeekNumber,
                            CourseName    = week.Course,
                            WeekStartDate = week.StartDate,
                            WeekEndDate   = week.EndDate,
                            StatusText    = phase.Status.ToString()
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MentorSpace] BuildAssigned error: {ex.Message}");
        }

        return rows.OrderBy(r => r.TraineeName).ThenBy(r => r.WeekNumber).ToList();
    }

    // ── Envoi de feedback — SANS TraineeId ───────────────────────────
    // Le mentor envoie un message général vers l'Admin.
    // Seul MentorId est obligatoire.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendFeedback(string message, int mentorId)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            SetError("Le message ne peut pas être vide.");
            return RedirectToAction(nameof(Index), new { mentorId });
        }

        var dto = new InterManagement.Application.Features.Feedbacks.DTOs.CreateFeedbackDto
        {
            Message   = message.Trim(),
            MentorId  = mentorId,
            TraineeId = null  // pas de stagiaire ciblé
        };

        var created = await _feedbackService.CreateAsync(dto);

        if (created is null)
            SetError("Échec de l'envoi du message.");
        else
            SetSuccess("Message envoyé avec succès.");

        return RedirectToAction(nameof(Index), new { mentorId });
    }

    // ── Suppression de feedback ───────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFeedback(int id, int mentorId)
    {
        var success = await _feedbackService.DeleteAsync(id);

        if (!success)
            SetError("Échec de la suppression.");
        else
            SetSuccess("Message supprimé.");

        return RedirectToAction(nameof(Index), new { mentorId });
    }
}