using SL.Application.Interfaces.Services.Caching;

namespace SL.Infrastructure.Services.Caching;

public class MemoryCacheTokenService : IMemoryCacheTokenService
{
    private CancellationTokenSource _tokenSource = new();
    public CancellationTokenSource TokenSource => _tokenSource;

    public void CancelCurrentToken()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
        _tokenSource = new CancellationTokenSource();
    }
}