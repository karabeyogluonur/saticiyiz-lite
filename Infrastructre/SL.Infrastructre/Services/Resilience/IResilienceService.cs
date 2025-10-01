namespace SL.Infrastructre.Services.Resilience
{
    public interface IResilienceService
    {
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName);
        Task<T> ExecuteWithCircuitBreakerAsync<T>(Func<Task<T>> operation, string operationName);
    }
}
