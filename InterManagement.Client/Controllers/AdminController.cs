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
            SetError("Failed to create admin. The email may already be in use.");
            return View(model);
        }

        SetSuccess($"Admin {created.FirstName} {created.LastName} created successfully.");
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
            SetError("Failed to update admin. Please verify the data and try again.");
            ViewBag.AdminId = id;
            return View(model);
        }

        SetSuccess($"Admin {updated.FirstName} {updated.LastName} updated successfully.");
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
            SetError("Failed to delete admin.");
            return RedirectToAction(nameof(Delete), new { id });
        }

        SetSuccess("Admin deleted successfully.");
        return RedirectToAction(nameof(Index));
    }
}