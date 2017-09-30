using Microsoft.Extensions.HealthChecks;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace HealthCheckSample.HealthChecks
{
    public class RandomFailHealthCheck : IHealthCheck
    {
        private readonly Random _random;

        public RandomFailHealthCheck()
        {
            _random = new Random();
        }

        public ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckStatus status = (CheckStatus)_random.Next(1, 3);
            return new ValueTask<IHealthCheckResult>(HealthCheckResult.FromStatus(status, $"Failed random"));
        }
    }
}
