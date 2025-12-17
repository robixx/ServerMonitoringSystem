using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenSysMonitor.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController] 
    [Authorize]

    public class DashboardController : Controller
    {
        [HttpGet("get_result")]
        public IActionResult Index()
        {
            return Ok(new { message = "Dashboard result" });
        }
    }
}
