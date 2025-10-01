using SL.Application.Models.DTOs.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface ITokenFactoryService
{
    List<TokenModel> GetAllTokens();

    List<TokenModel> GetTokensForEntity(object entity);

    List<TokenModel> GetSystemTokens();

    List<TokenModel> GetEntityTokens(object entity);

    List<TokenModel> GetTokensByCategory(string category);
}
