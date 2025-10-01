using SL.Application.Models.DTOs.Messages;
using SL.Domain.Entities.Messages;

namespace SL.Application.Interfaces.Services.Messages;

public interface IMessageTemplateService
{
    Task<string> ReplaceTokensAsync(string template, IList<TokenModel> tokens);

    Task<string> ReplaceTokensAsync(string template, params object[] entities);

    Task<string> ReplaceTokensForSystemAsync(string systemName, string template, params object[] entities);

    Task<EmailTemplate?> GetMessageTemplateAsync(string systemName);


    Task<IEnumerable<string>> GetAllAvailableTokensAsync();

    Task<IEnumerable<string>> GetAvailableTokensForSystemAsync(string systemName);
}
