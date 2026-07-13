using InterManagement.Application.Features.Feedbacks.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class FeedbackController : BaseController
{
    private readonly IFeedbackApiService _feedbackService;

    public FeedbackController(IFeedbackApiService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    public async Task<IActionResult> Index(int? traineeId = null)
    {
        var feedbacks = traineeId.HasValue
            ? await _feedbackService.GetByTraineeAsync(traineeId.Value)
            : await _feedbackService.GetAllAsync();

        ViewBag.CurrentTraineeId = traineeId;
        return View(feedbacks);
    }

    // Endpoint JSON appelé par site.js pour les notifications admin
    [HttpGet("/api-client/feedback")]
    public async Task<IActionResult> GetAllJson()
    {
        var feedbacks = await _feedbackService.GetAllAsync();
        return Json(feedbacks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback is null) return NotFound();
        return View(feedback);
    }

    public IActionResult Create() => View(new CreateFeedbackDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFeedbackDto model)
    {
        var created = await _feedbackService.CreateAsync(model);
        if (created is null)
        {
            SetError("Failed to send feedback. Please verify the trainee.");
            return View(model);
        }

        SetSuccess("Feedback sent successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback is null) return NotFound();

        var dto = new UpdateFeedbackDto
        {
            Message = feedback.Message
        };

        ViewBag.FeedbackId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateFeedbackDto model)
    {
        var updated = await _feedbackService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Failed to update feedback.");
            ViewBag.FeedbackId = id;
            return View(model);
        }

        SetSuccess("Feedback updated successfully.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var feedback = await _feedbackService.GetByIdAsync(id);
        if (feedback is null) return NotFound();
        return View(feedback);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _feedbackService.DeleteAsync(id);
        if (!success)
        {
            SetError("Failed to delete feedback.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Feedback deleted successfully.");
        return RedirectToAction(nameof(Index));
    }
}
