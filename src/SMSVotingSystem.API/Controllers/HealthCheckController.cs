using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMSVotingSystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            var response = new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(e => new
                {
                    Component = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description,
                    Duration = e.Value.Duration.ToString()
                }),
                TotalDuration = report.TotalDuration.ToString()
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(response)
                : StatusCode((int)HttpStatusCode.ServiceUnavailable, response);
        }

        [HttpGet("database")]
        public async Task<IActionResult> Database()
        {
            var report = await _healthCheckService.CheckHealthAsync(c => c.Tags.Contains("database"));
            
            return report.Status == HealthStatus.Healthy
                ? Ok("Database is healthy")
                : StatusCode((int)HttpStatusCode.ServiceUnavailable, "Database is unhealthy");
        }

        [HttpGet("sms")]
        public async Task<IActionResult> SmsService()
        {
            var report = await _healthCheckService.CheckHealthAsync(c => c.Tags.Contains("sms"));
            
            return report.Status == HealthStatus.Healthy
                ? Ok("SMS service is healthy")
                : StatusCode((int)HttpStatusCode.ServiceUnavailable, "SMS service is unhealthy");
        }
    }
}