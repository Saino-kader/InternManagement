using InterManagement.Application.Features.Admins.DTOs;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;

public class AdminController : BaseController
{
    private readonly IAdminApiService _adminService;

    public AdminController(IAdminApiService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var admins = await _adminService.GetAllAsync();
        return View(admins);
    }

    public async Task<IActionResult> Details(int id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        if (admin is null) return NotFound();
        return View(admin);
    }

    public IActionResult Create() => View(new CreateAdminDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAdminDto model)
    {
        var created = await _adminService.CreateAsync(model);
        if (created is null)
        {
            SetError("Échec de la création de l'administrateur. L'email est peut-être déjà utilisé.");
            return View(model);
        }

        SetSuccess($"Administrateur {created.FirstName} {created.LastName} créé avec succès.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        if (admin is null) return NotFound();

        var dto = new UpdateAdminDto
        {
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Email = admin.Email
        };

        ViewBag.AdminId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateAdminDto model)
    {
        var updated = await _adminService.UpdateAsync(id, model);
        if (updated is null)
        {
            SetError("Échec de la modification de l'administrateur. Vérifiez les données et réessayez.");
            ViewBag.AdminId = id;
            return View(model);
        }

        SetSuccess($"Administrateur {updated.FirstName} {updated.LastName} modifié avec succès.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var admin = await _adminService.GetByIdAsync(id);
        if (admin is null) return NotFound();
        return View(admin);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _adminService.DeleteAsync(id);
        if (!success)
        {
            SetError("Échec de la suppression de l'administrateur.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Administrateur supprimé avec succès.");
        return RedirectToAction(nameof(Index));
    }
}