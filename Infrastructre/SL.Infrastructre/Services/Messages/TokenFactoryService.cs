using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.DTOs.Messages;

namespace SL.Infrastructre.Services.Messages;

public class TokenFactoryService : ITokenFactoryService
{
    private readonly IEnumerable<ITokenProvider> _providers;

    public TokenFactoryService(IEnumerable<ITokenProvider> providers)
    {
        _providers = providers;
    }

    public List<TokenModel> GetAllTokens()
    {
        var tokens = new List<TokenModel>();

        foreach (var provider in _providers)
        {
            var providerTokens = provider.GetTokensForEntity(null);
            tokens.AddRange(providerTokens);
        }

        return tokens;
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
        return GetAllTokens().Where(t => t.IsSystemToken).ToList();
    }

    public List<TokenModel> GetEntityTokens(object entity)
    {
        return GetTokensForEntity(entity).Where(t => !t.IsSystemToken).ToList();
    }

    public List<TokenModel> GetTokensByCategory(string category)
    {
        return GetAllTokens().Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
