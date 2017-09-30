using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace HealthCheckSample.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHealthCheckService _healthCheckService;

        public ValuesController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // manually check
            var healthCheckResult = await _healthCheckService.CheckHealthAsync();

            bool errorOccurred = healthCheckResult.CheckStatus != CheckStatus.Healthy;

            if (errorOccurred)
            {
                // return unhealthy checks
                var errorDescriptions = healthCheckResult.Results.Where(r => r.Value.CheckStatus != CheckStatus.Healthy)
                                                                 .Select(r => r.Value.Description)
                                                                 .ToList();

                return new JsonResult(new { Errors = errorDescriptions }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return Ok(new string[] { "value1", "value2" });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
