using InterManagement.Application.Features.Mentors.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class MentorController : BaseController
{
    private readonly IMentorApiService _mentorService;

    public MentorController(IMentorApiService mentorService)
    {
        _mentorService = mentorService;
    }



    public async Task<IActionResult> Index(string? department = null)
    {
        var mentors = !string.IsNullOrWhiteSpace(department)
            ? await _mentorService.GetByDepartmentAsync(department)
            : await _mentorService.GetAllAsync();

        ViewBag.CurrentDepartment = department;
        return View(mentors);
    }

    public async Task<IActionResult> Details(int id)
    {
        var mentor = await _mentorService.GetDetailsAsync(id);
        if (mentor is null) return NotFound();
        return View(mentor);
    }

    public IActionResult Create() => View(new CreateMentorDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMentorDto model)
    {
        var created = await _mentorService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création du mentor. L'email est peut-être déjà utilisé.");
            return View(model);
        }

        SetSuccess($"Mentor {created.FirstName} {created.LastName} créé avec succès.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor is null) return NotFound();

        var dto = new UpdateMentorDto
        {
            FirstName = mentor.FirstName,
            LastName = mentor.LastName,
            Email = mentor.Email,
            Department = mentor.Department,
            Specialty = mentor.Specialty
        };

        ViewBag.MentorId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateMentorDto model)
    {
        var updated = await _mentorService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Échec de la modification du mentor. Vérifiez les données et réessayez.");
            ViewBag.MentorId = id;
            return View(model);
        }

        SetSuccess($"Mentor {updated.FirstName} {updated.LastName} modifié avec succès.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var mentor = await _mentorService.GetByIdAsync(id);
        if (mentor is null) return NotFound();
        return View(mentor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _mentorService.DeleteAsync(id);
        if (!success)
        {
            SetError("Échec de la suppression du mentor.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Mentor supprimé avec succès.");
        return RedirectToAction(nameof(Index));
    }
}
