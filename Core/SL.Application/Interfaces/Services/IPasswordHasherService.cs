using System;

namespace SL.Application.Interfaces.Services;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string providedPassword, string hashedPassword);
}
