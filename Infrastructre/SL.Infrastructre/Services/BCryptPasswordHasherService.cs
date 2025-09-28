using SL.Application.Interfaces.Services;
using BCryptNet = BCrypt.Net.BCrypt;
namespace SL.Infrastructre.Services;

public class BCryptPasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }
    public bool VerifyPassword(string providedPassword, string hashedPassword)
    {
        try
        {
            return BCryptNet.Verify(providedPassword, hashedPassword);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
