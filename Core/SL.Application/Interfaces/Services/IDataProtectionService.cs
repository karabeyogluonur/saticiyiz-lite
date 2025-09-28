using System;

namespace SL.Application.Interfaces.Services;

public interface IDataProtectionService
{
    string Encrypt(string plainText);

    string? Decrypt(string cipherText);
}
