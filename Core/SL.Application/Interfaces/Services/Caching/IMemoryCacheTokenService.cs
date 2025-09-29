namespace SL.Application.Interfaces.Services.Caching;

public interface IMemoryCacheTokenService
{
    CancellationTokenSource TokenSource { get; }
    void CancelCurrentToken();
}