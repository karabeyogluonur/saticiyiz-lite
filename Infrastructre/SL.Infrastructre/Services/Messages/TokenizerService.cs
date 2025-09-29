using SL.Application.Interfaces.Services.Messages;

namespace SL.Infrastructre.Services.Messages;

public class TokenizerService : ITokenizerService
{
    private readonly IEnumerable<IMessageTokenProvider> _tokenProviders;

    public TokenizerService(IEnumerable<IMessageTokenProvider> tokenProviders)
    {
        _tokenProviders = tokenProviders;
    }

    public IEnumerable<string> GetAllAllowedTokens()
    {
        return _tokenProviders.SelectMany(p => p.GetAllowedTokens()).Distinct().OrderBy(s => s);
    }

    public string Replace(string template, params object[] data)
    {
        var allTokenValues = new Dictionary<string, string>();


        foreach (var provider in _tokenProviders)
        {
            provider.AddTokenValues(allTokenValues, data);
        }


        foreach (var kvp in allTokenValues)
        {
            template = template.Replace(kvp.Key, kvp.Value);
        }

        return template;
    }
}
