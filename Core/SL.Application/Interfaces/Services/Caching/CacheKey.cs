using System;
using System.Linq;

namespace SL.Application.Interfaces.Services.Caching;

public class CacheKey
{
    public string Key { get; protected set; }
    public string Prefix { get; protected set; }
    public int? CacheTimeInMinutes { get; set; }



    public CacheKey(string keyFormat, params object[] args)
    {
        Key = string.Format(keyFormat, args.Select(a => a?.ToString() ?? "null").ToArray());
        Prefix = keyFormat.Split(':')[0];
    }

    private CacheKey(string key, string prefix)
    {
        Key = key;
        Prefix = prefix;
    }

    public static CacheKey WithPrefix(string key, string prefix)
    {
        return new CacheKey(key, prefix);
    }
}