using FileFlowApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace FileFlowApi.Controllers
{

    [Route("api/health")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly FileFlowDbContext _context;

        public HealthCheckController(FileFlowDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-db")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                return Ok(new { success = canConnect });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

}

