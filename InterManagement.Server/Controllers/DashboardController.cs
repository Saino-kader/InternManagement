// Server/Controllers/DashboardController.cs
using InterManagement.Application.Features.Dashboard.Queries.GetDashboardStats;
using InterManagement.Application.Features.Dashboard.Queries.GetRecentActivity;
using Microsoft.AspNetCore.Mvc;

namespace InterManagement.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly GetDashboardStatsHandler _statsHandler;
        private readonly GetRecentActivityHandler _activityHandler;

        public DashboardController(
            GetDashboardStatsHandler statsHandler,
            GetRecentActivityHandler activityHandler)
        {
            _statsHandler = statsHandler;
            _activityHandler = activityHandler;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _statsHandler.Handle(new GetDashboardStatsQuery());
            return Ok(result);
        }

        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity([FromQuery] int count = 10)
        {
            var query = new GetRecentActivityQuery { Count = count };
            var result = await _activityHandler.Handle(query);
            return Ok(result);
        }
    }
}
