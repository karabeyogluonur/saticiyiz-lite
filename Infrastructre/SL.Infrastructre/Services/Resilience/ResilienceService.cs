using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace SL.Infrastructre.Services.Resilience
{
    public class ResilienceService : IResilienceService
    {
        private readonly ILogger<ResilienceService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public ResilienceService(ILogger<ResilienceService> logger)
        {
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Retry {RetryCount} for {OperationName} in {Delay}ms",
                            retryCount, context.OperationKey, timespan.TotalMilliseconds);
                    });

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(1),
                    onBreak: (exception, duration) =>
                    {
                        _logger.LogError("Circuit breaker opened for {Duration}ms", duration.TotalMilliseconds);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation("Circuit breaker reset");
                    });
        }

        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await operation();
            });
        }

        public async Task<T> ExecuteWithCircuitBreakerAsync<T>(Func<Task<T>> operation, string operationName)
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await operation();
            });
        }
    }
}
