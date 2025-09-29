using SL.Application.Interfaces.Services.Caching;
using SL.Domain.Entities.Membership; // ApplicationUser entity'nizin bulunduğu namespace

namespace SL.Application.Defaults.Caching;

/// <summary>
/// ApplicationUser entity'si için standart cache anahtarlarını ve şablonlarını sağlar.
/// Bu sınıf, uygulama genelinde tutarlılığı sağlamak için kullanılır.
/// </summary>
public static class UserCacheDefaults
{
    /// <summary>
    /// ApplicationUser entity'si için kullanılan ön ek (prefix). Varsayılan olarak "ApplicationUser"dır.
    /// Entity ismini doğru kullandığınızdan emin olun.
    /// </summary>
    public static string Prefix => EntityCacheDefaults<ApplicationUser>.Prefix;

    /// <summary>
    /// Bir kullanıcıyı birincil anahtarına (ID) göre getiren cache anahtarı.
    /// Format: "ApplicationUser:{id}"
    /// </summary>
    /// <param name="id">Kullanıcının birincil anahtarı (Primary Key)</param>
    public static CacheKey ByIdCacheKey(Guid id) => EntityCacheDefaults<ApplicationUser>.ByIdCacheKey(id);

    /// <summary>
    /// Tüm kullanıcıları içeren bir liste için cache anahtarı.
    /// Format: "ApplicationUser:All"
    /// </summary>
    public static CacheKey AllCacheKey => EntityCacheDefaults<ApplicationUser>.AllCacheKey;

    /// <summary>
    /// Bir kullanıcının e-posta adresine göre ID'sini getiren cache anahtarı.
    /// E-posta adresleri büyük/küçük harf duyarlı olmaması için anahtar oluşturulurken küçük harfe çevrilir.
    /// Format: "ApplicationUser:IdByEmail:{email_lowercase}"
    /// </summary>
    /// <param name="email">Kullanıcının e-posta adresi</param>
    public static CacheKey IdByEmailCacheKey(string email) =>
        new CacheKey($"{Prefix}:IdByEmail:{{0}}", email.ToLowerInvariant());

    /// <summary>
    /// Bir kullanıcıyı kullanıcı adına göre getiren cache anahtarı.
    /// Kullanıcı adları da genellikle büyük/küçük harf duyarsız olarak ele alınır.
    /// Format: "ApplicationUser:ByUsername:{username_lowercase}"
    /// </summary>
    /// <param name="username">Kullanıcı adı</param>
    public static CacheKey ByUsernameCacheKey(string username) =>
        new CacheKey($"{Prefix}:ByUsername:{{0}}", username.ToLowerInvariant());
}