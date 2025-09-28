using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using SL.Application.Interfaces.Services;
using System.Security.Cryptography;

namespace SL.Infrastructure.Security
{
    public class DataProtectionService : IDataProtectionService
    {
        private readonly IDataProtector _protector;
        private readonly ILogger<DataProtectionService> _logger;

        // DI container'dan IDataProtectionProvider'ı talep ediyoruz.
        public DataProtectionService(IDataProtectionProvider provider, ILogger<DataProtectionService> logger)
        {
            _logger = logger;
            _protector = provider.CreateProtector("Saticiyiz.EmailAccount.Password.v1");
        }

        public string Encrypt(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string? Decrypt(string cipherText)
        {
            try
            {
                return _protector.Unprotect(cipherText);
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex, "Veri deşifre edilirken kriptografik bir hata oluştu.");
                return null;
            }
        }
    }
}