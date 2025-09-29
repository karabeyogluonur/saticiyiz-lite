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

        // Her provider'a token değerlerini üretmesi için şans ver.
        foreach (var provider in _tokenProviders)
        {
            provider.AddTokenValues(allTokenValues, data);
        }

        // Toplanan tüm token'ları metin içinde değiştir.
        foreach (var kvp in allTokenValues)
        {
            template = template.Replace(kvp.Key, kvp.Value);
        }

        return template;
    }
}
