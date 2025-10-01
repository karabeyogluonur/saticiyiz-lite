using SL.Application.Models.DTOs.Messages;
using SL.Domain.Entities.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface ITokenProvider
{
    IEnumerable<string> GetTokens();

    List<TokenModel> GetTokensForEntity(object entity);

    bool CanHandle(object entity);
}
