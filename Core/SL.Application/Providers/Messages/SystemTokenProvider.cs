using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.DTOs.Messages;

namespace SL.Application.Providers.Messages;

public class SystemTokenProvider : ITokenProvider
{
    public IEnumerable<string> GetTokens()
    {
        return new[]
        {
            "System.Name",
            "System.Version",
            "System.Url",
            "System.SupportEmail",
            "System.SupportPhone",
            "System.CurrentDate",
            "System.CurrentTime",
            "System.CurrentDateTime",
            "System.Year",
            "System.Month",
            "System.Day"
        };
    }

    public List<TokenModel> GetTokensForEntity(object entity)
    {
        var tokens = new List<TokenModel>();
        var now = DateTime.UtcNow;

        tokens.Add(new TokenModel { Name = "System.Name", Value = "SL Application", Description = "System name", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.Version", Value = "1.0.0", Description = "System version", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.Url", Value = "https://slapp.com", Description = "System URL", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.SupportEmail", Value = "support@slapp.com", Description = "Support email", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.SupportPhone", Value = "+90 212 555 0123", Description = "Support phone", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.CurrentDate", Value = now.ToString("yyyy-MM-dd"), Description = "Current date", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.CurrentTime", Value = now.ToString("HH:mm:ss"), Description = "Current time", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.CurrentDateTime", Value = now.ToString("yyyy-MM-dd HH:mm:ss"), Description = "Current date and time", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.Year", Value = now.Year.ToString(), Description = "Current year", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.Month", Value = now.Month.ToString(), Description = "Current month", Category = "System", IsSystemToken = true });
        tokens.Add(new TokenModel { Name = "System.Day", Value = now.Day.ToString(), Description = "Current day", Category = "System", IsSystemToken = true });

        return tokens;
    }

    public bool CanHandle(object entity)
    {
        return true;
    }
}
