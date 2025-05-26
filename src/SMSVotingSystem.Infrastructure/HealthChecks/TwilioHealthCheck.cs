using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Twilio;
using Twilio.Rest.Api.V2010;
using Twilio.Rest.Api.V2010.Account;

namespace SMSVotingSystem.Infrastructure.HealthChecks
{
    public class TwilioHealthCheck : IHealthCheck
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        
        public TwilioHealthCheck(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"] ?? throw new ArgumentNullException("Twilio Account SID is not configured.");
            _authToken = configuration["Twilio:AuthToken"] ?? throw new ArgumentNullException("Twilio Auth Token is not configured.");
        }
        
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Initialize Twilio client
                TwilioClient.Init(_accountSid, _authToken);
                
                // Try to fetch account info to verify credentials are valid
                var account = await AccountResource.FetchAsync();
                
                if (account != null)
                {
                    return HealthCheckResult.Healthy($"Twilio connection is healthy. Account: {account.FriendlyName}");
                }
                
                return HealthCheckResult.Degraded("Twilio connection check returned unexpected results");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Twilio connection failed", ex);
            }
        }
    }
}