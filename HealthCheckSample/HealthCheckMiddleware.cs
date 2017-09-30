using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.HealthChecks;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheckSample
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckMiddleware(RequestDelegate next, IHealthCheckService healthCheckService)
        {
            _next = next;
            _healthCheckService = healthCheckService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var healthCheckResult = await _healthCheckService.CheckHealthAsync();

            bool errorOccurred = healthCheckResult.CheckStatus != CheckStatus.Healthy;

            if (errorOccurred)
            {
                var errorDescriptions = healthCheckResult.Results.Where(r => r.Value.CheckStatus != CheckStatus.Healthy)
                                                                     .Select(r => r.Value.Description)
                                                                     .ToList();

                // return 500 with error descriptions
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { Errors = errorDescriptions }));
                return;
            }

            await _next(httpContext);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }
    }
}