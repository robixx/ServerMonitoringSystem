using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysMonitor.Application.IInterface;
using System.Net.NetworkInformation;

namespace OpenSysMonitor.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class MonitorController : ControllerBase
    {

        private readonly ISystemMonitorService _systemMonitorService;

        public MonitorController(ISystemMonitorService systemMonitorService)
        {
            _systemMonitorService = systemMonitorService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetSystemStatus()
        {
            var fulldata= await _systemMonitorService.GetSystemStatusAsync();
            return Ok(fulldata);
        }
    }

}
