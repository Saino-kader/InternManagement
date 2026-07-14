// Controllers/DashboardController.cs
using InterManagement.Client.Models;
using InterManagement.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Client.Controllers;
public class DashboardController : BaseController
{
    private readonly IDashboardApiService _dashboardService;

    public DashboardController(IDashboardApiService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        // Seul Admin peut accéder au Dashboard
        var check = RequireRole("Admin");
        if (check != null) return check;

        var statsTask    = _dashboardService.GetStatsAsync();
        var activityTask = _dashboardService.GetRecentActivityAsync(10);

        await Task.WhenAll(statsTask, activityTask);

        var model = new DashboardViewModel
        {
            Stats            = await statsTask ?? new(),
            RecentActivities = await activityTask
        };

        return View(model);
    }
}
