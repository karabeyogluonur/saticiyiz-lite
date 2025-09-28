using System;
using SL.Application.Interfaces.Services;
using SL.Domain.Entities;

namespace SL.Infrastructre.Services;

public class CustomerTokenProvider : IMessageTokenProvider
{
    public IEnumerable<string> GetAllowedTokens()
    {
        return new[] { "{{User.FullName}}", "{{User.Email}}" };
    }

    public void AddTokenValues(Dictionary<string, string> tokenValues, params object[] data)
    {
        var user = data.OfType<ApplicationUser>().FirstOrDefault();

        if (user != null)
        {
            tokenValues["{{User.FullName}}"] = $"{user.FirstName} {user.LastName}";
            tokenValues["{{User.Email}}"] = user.Email;
        }
    }
}
