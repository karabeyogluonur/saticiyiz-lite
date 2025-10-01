using SL.Application.Interfaces.Services.Messages;
using SL.Application.Models.DTOs.Messages;
using SL.Domain.Entities.Membership;

namespace SL.Application.Providers.Messages;

public class UserTokenProvider : ITokenProvider
{
    public IEnumerable<string> GetTokens()
    {
        return new[]
        {
            "User.Id",
            "User.FirstName",
            "User.LastName",
            "User.FullName",
            "User.Email",
            "User.PhoneNumber",
            "User.TenantId",
            "User.TenantName"
        };
    }

    public List<TokenModel> GetTokensForEntity(object entity)
    {
        var tokens = new List<TokenModel>();

        if (entity is ApplicationUser user)
        {
            tokens.Add(new TokenModel { Name = "User.Id", Value = user.Id.ToString(), Description = "User ID", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.FirstName", Value = user.FirstName ?? "", Description = "User first name", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.LastName", Value = user.LastName ?? "", Description = "User last name", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.FullName", Value = $"{user.FirstName} {user.LastName}".Trim(), Description = "User full name", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.Email", Value = user.Email ?? "", Description = "User email", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.PhoneNumber", Value = user.PhoneNumber ?? "", Description = "User phone number", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.TenantId", Value = user.TenantId.ToString(), Description = "User tenant ID", Category = "User" });
            tokens.Add(new TokenModel { Name = "User.TenantName", Value = user.Tenant?.DatabaseName ?? "", Description = "User tenant name", Category = "User" });
        }

        return tokens;
    }

    public bool CanHandle(object entity)
    {
        return entity is ApplicationUser;
    }
}
