namespace SL.Application.Interfaces.Services.Caching;


// Sadece In-Memory Cache'i temsil eden arayüz.
// Bu, L1 cache'e doğrudan erişim gerektiğinde kullanılabilir.
public interface IStaticCacheManager : ICacheManager
{
}