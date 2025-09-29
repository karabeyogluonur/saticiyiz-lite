using System;
namespace SL.Application.Interfaces.Services.Security;

public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string providedPassword, string hashedPassword);
}
