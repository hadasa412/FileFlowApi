using core.IServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FILEFLOWAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var report = await _reportService.GetSummaryReportAsync();
            return Ok(report);
        }
        [HttpGet("uploads-per-day")]
        public async Task<IActionResult> GetUploadsPerDay()
        {
            var uploads = await _reportService.GetUploadsPerDayAsync();
            return Ok(uploads);
        }


    }

}

