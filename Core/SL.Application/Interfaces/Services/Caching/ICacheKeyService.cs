using SL.Application.Interfaces.Services.Caching;

namespace SL.Application.Interfaces.Services.Caching;

public interface ICacheKeyService
{
    /// <summary>
    /// Belirtilen ID'ye sahip bir entity için cache anahtarı hazırlar.
    /// </summary>
    CacheKey PrepareKeyFor<TEntity>(object id) where TEntity : class;

    /// <summary>
    /// Belirli bir entity tipinin tümünü temsil eden liste için anahtar hazırlar.
    /// </summary>
    CacheKey PrepareKeyForList<TEntity>() where TEntity : class;

    /// <summary>
    /// Belirtilen entity tipi için cache prefix'ini döner.
    /// (RemoveByPrefixAsync gibi operasyonlar için kullanışlıdır.)
    /// </summary>
    string GetPrefix<TEntity>() where TEntity : class;
}