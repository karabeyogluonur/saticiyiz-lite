using System;

namespace SL.Application.Interfaces.Services;

public interface IMessageTokenProvider
{
    IEnumerable<string> GetAllowedTokens();
    void AddTokenValues(Dictionary<string, string> tokenValues, params object[] data);
}