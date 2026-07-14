// Controllers/MentorSpaceController.cs
using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class MentorSpaceController : BaseController
{
    private readonly IMentorApiService _mentorService;
    private readonly IAssignmentApiService _assignmentService;
    private readonly ITraineeApiService _traineeService;
    private readonly IFeedbackApiService _feedbackService;
    private readonly ILogger<MentorSpaceController> _logger;

    public MentorSpaceController(
        IMentorApiService mentorService,
        IAssignmentApiService assignmentService,
        ITraineeApiService traineeService,
        IFeedbackApiService feedbackService,
        ILogger<MentorSpaceController> logger)
    {
        _mentorService     = mentorService;
        _assignmentService = assignmentService;
        _traineeService    = traineeService;
        _feedbackService   = feedbackService;
        _logger            = logger;
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
    // Un seul appel réseau (assignment/mentor/{id}/details, qui charge
    // Trainee/Phase/Weeks/WeeklyFollowUps en une seule requête EF côté
    // API) au lieu d'une boucle Trainee+Phase+Week par assignation.
    private async Task<List<MentorAssignedTraineeItem>> BuildAssignedTraineesAsync(
        int mentorId)
    {
        var rows = new List<MentorAssignedTraineeItem>();
        try
        {
            var assignments = await _assignmentService.GetMentorAssignmentsDetailsAsync(mentorId);
            if (assignments == null || !assignments.Any()) return rows;

            foreach (var assignment in assignments.Where(a => a.IsActive))
            {
                if (!assignment.Weeks.Any())
                {
                    rows.Add(new MentorAssignedTraineeItem
                    {
                        TraineeId     = assignment.TraineeId,
                        TraineeName   = assignment.TraineeName,
                        PhaseTitle    = assignment.PhaseTitle,
                        PhaseNumber   = assignment.PhaseNumber?.ToString() ?? string.Empty,
                        WeekNumber    = 0,
                        CourseName    = "—",
                        WeekStartDate = null,
                        WeekEndDate   = null,
                        StatusText    = assignment.PhaseStatus
                    });
                }
                else
                {
                    foreach (var week in assignment.Weeks.OrderBy(w => w.WeekNumber))
                    {
                        rows.Add(new MentorAssignedTraineeItem
                        {
                            TraineeId     = assignment.TraineeId,
                            TraineeName   = assignment.TraineeName,
                            PhaseTitle    = assignment.PhaseTitle,
                            PhaseNumber   = assignment.PhaseNumber?.ToString() ?? string.Empty,
                            WeekNumber    = week.WeekNumber,
                            CourseName    = week.Course,
                            WeekStartDate = week.StartDate,
                            WeekEndDate   = week.EndDate,
                            StatusText    = assignment.PhaseStatus
                        });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building assigned trainees for mentor {MentorId}", mentorId);
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

        var dto = new CreateFeedbackDto
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