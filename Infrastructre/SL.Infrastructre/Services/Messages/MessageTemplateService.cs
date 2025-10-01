using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.DTOs.Messages;
using SL.Application.Providers.Messages;
using SL.Domain.Entities.Membership;
using SL.Domain.Entities.Messages;

namespace SL.Infrastructre.Services.Messages;

    public class MessageTemplateService : IMessageTemplateService
    {
        private static readonly Regex TokenRegex = new Regex("\\{\\{([A-Za-z0-9\\._\\-]+)\\}\\}", RegexOptions.Compiled);

    private readonly ITokenFactoryService _tokenFactoryService;
    private readonly ITokenRegistryService _tokenRegistryService;
    private readonly ILogger<MessageTemplateService> _logger;
    private readonly IEmailTemplateService _emailTemplateService;

public MessageTemplateService(
    ITokenFactoryService tokenFactoryService, 
    ITokenRegistryService tokenRegistryService, 
    ILogger<MessageTemplateService> logger,
    IEmailTemplateService emailTemplateService)
{
    _tokenFactoryService = tokenFactoryService;
    _tokenRegistryService = tokenRegistryService;
    _logger = logger;
    _emailTemplateService = emailTemplateService;
}

    public async Task<string> ReplaceTokensAsync(string template, IList<TokenModel> tokens)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var tokenMap = tokens.ToDictionary(t => $"{{{{{t.Name}}}}}", t => t.Value, StringComparer.OrdinalIgnoreCase);

        string result = TokenRegex.Replace(template, match =>
        {
            string tokenName = match.Groups[1].Value;
            string tokenKey = $"{{{{{tokenName}}}}}";

            if (tokenMap.TryGetValue(tokenKey, out var value))
            {
                return value;
            }

            _logger.LogWarning("Token not found: {TokenName}", tokenName);
            return match.Value; // Keep original token if not found
        });

        return result;
    }

    public async Task<string> ReplaceTokensAsync(string template, params object[] entities)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var tokens = new List<TokenModel>();
        foreach (var entity in entities)
        {
            var entityTokens = _tokenFactoryService.GetTokensForEntity(entity);
            tokens.AddRange(entityTokens);
        }
        
        var systemTokens = _tokenFactoryService.GetSystemTokens();
        tokens.AddRange(systemTokens);

        return await ReplaceTokensAsync(template, tokens);
    }

    public async Task<string> ReplaceTokensForSystemAsync(string systemName, string template, params object[] entities)
    {
        var tokens = new List<TokenModel>();
        foreach (var entity in entities)
        {
            var entityTokens = _tokenFactoryService.GetTokensForEntity(entity);
            tokens.AddRange(entityTokens);
        }
        
        var systemTokens = _tokenFactoryService.GetSystemTokens();
        tokens.AddRange(systemTokens);
        
        return await ReplaceTokensAsync(template, tokens);
    }

    public async Task<EmailTemplate?> GetMessageTemplateAsync(string systemName)
    {
        try
        {
            _logger.LogInformation("Getting message template for system: {SystemName}", systemName);
            
            var template = await _emailTemplateService.GetTemplateBySystemNameAsync(systemName);
            
            if (template == null)
            {
                _logger.LogWarning("Template not found for system: {SystemName}", systemName);
                return null;
            }
            
            _logger.LogInformation("Found template for system: {SystemName}, EmailAccountId: {EmailAccountId}", 
                systemName, template.EmailAccountId);
            
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting message template for system: {SystemName}", systemName);
            return null;
        }
    }


    public async Task<IEnumerable<string>> GetAllAvailableTokensAsync()
    {
        return await Task.FromResult(_tokenRegistryService.GetAllAvailableTokens());
    }

    public async Task<IEnumerable<string>> GetAvailableTokensForSystemAsync(string systemName)
    {
        return await Task.FromResult(_tokenRegistryService.GetTokensForSystem(systemName));
    }
}
