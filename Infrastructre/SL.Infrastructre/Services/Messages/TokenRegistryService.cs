using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.DTOs.Messages;
using SL.Domain.Defaults.Messages;

namespace SL.Infrastructre.Services.Messages;

public class TokenRegistryService : ITokenRegistryService
{
    private readonly List<ITokenProvider> _providers;
    private readonly Dictionary<string, List<Type>> _systemMappings;

    public TokenRegistryService(IEnumerable<ITokenProvider> tokenProviders)
    {
        _providers = new List<ITokenProvider>();
        _systemMappings = GetSystemMappings();
        
        foreach (var provider in tokenProviders)
        {
            RegisterTokenProvider(provider);
        }
    }

    public void RegisterTokenProvider(ITokenProvider provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        if (!_providers.Contains(provider))
        {
            _providers.Add(provider);
        }
    }

    public IEnumerable<ITokenProvider> GetTokenProvidersForSystem(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return Enumerable.Empty<ITokenProvider>();

        if (_systemMappings.TryGetValue(systemName, out var entityTypes))
        {
            return _providers.Where(p => entityTypes.Any(et => p.CanHandle(Activator.CreateInstance(et) ?? new object())));
        }

        return Enumerable.Empty<ITokenProvider>();
    }

    public IEnumerable<string> GetAllAvailableTokens()
    {
        var tokens = new HashSet<string>();
        
        foreach (var provider in _providers)
        {
            foreach (var token in provider.GetTokens())
            {
                tokens.Add(token);
            }
        }

        return tokens;
    }

    public IEnumerable<string> GetTokensForSystem(string systemName)
    {
        var providers = GetTokenProvidersForSystem(systemName);
        var tokens = new HashSet<string>();

        foreach (var provider in providers)
        {
            foreach (var token in provider.GetTokens())
            {
                tokens.Add(token);
            }
        }

        return tokens;
    }

    public IEnumerable<string> GetAvailableSystems()
    {
        return _systemMappings.Keys;
    }

    public List<TokenModel> GetTokensForEntity(object entity)
    {
        var tokens = new List<TokenModel>();

        foreach (var provider in _providers)
        {
            if (provider.CanHandle(entity))
            {
                var providerTokens = provider.GetTokensForEntity(entity);
                tokens.AddRange(providerTokens);
            }
        }

        return tokens;
    }

    public List<TokenModel> GetSystemTokens()
    {
        var tokens = new List<TokenModel>();

        foreach (var provider in _providers)
        {
            var providerTokens = provider.GetTokensForEntity(null);
            tokens.AddRange(providerTokens.Where(t => t.IsSystemToken));
        }

        return tokens;
    }

    public List<TokenModel> GetTokensByCategory(string category)
    {
        var tokens = new List<TokenModel>();

        foreach (var provider in _providers)
        {
            var providerTokens = provider.GetTokensForEntity(null);
            tokens.AddRange(providerTokens.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)));
        }

        return tokens;
    }

    public TokenModel? GetTokenByName(string tokenName)
    {
        foreach (var provider in _providers)
        {
            var tokens = provider.GetTokensForEntity(null);
            var token = tokens.FirstOrDefault(t => t.Name.Equals(tokenName, StringComparison.OrdinalIgnoreCase));
            if (token != null)
                return token;
        }

        return null;
    }

    private Dictionary<string, List<Type>> GetSystemMappings()
    {
        return new Dictionary<string, List<Type>>
        {
            { "User", new List<Type> { typeof(SL.Domain.Entities.Membership.ApplicationUser) } },
            { "System", new List<Type> { typeof(object) } } // System tokens are always available
        };
    }
}
