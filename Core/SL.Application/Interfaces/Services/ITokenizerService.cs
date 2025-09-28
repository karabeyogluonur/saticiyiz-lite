using System;

namespace SL.Application.Interfaces.Services;

public interface ITokenizerService
{
    string Replace(string template, params object[] data);
    IEnumerable<string> GetAllAllowedTokens();
}

