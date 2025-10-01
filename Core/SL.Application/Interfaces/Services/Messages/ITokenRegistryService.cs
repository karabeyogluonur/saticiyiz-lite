using SL.Application.Models.DTOs.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface ITokenRegistryService
{
    void RegisterTokenProvider(ITokenProvider provider);

    IEnumerable<ITokenProvider> GetTokenProvidersForSystem(string systemName);

    IEnumerable<string> GetAllAvailableTokens();

    IEnumerable<string> GetTokensForSystem(string systemName);

    IEnumerable<string> GetAvailableSystems();

    List<TokenModel> GetTokensForEntity(object entity);

    List<TokenModel> GetSystemTokens();

    List<TokenModel> GetTokensByCategory(string category);

    TokenModel? GetTokenByName(string tokenName);
}
