using SL.Application.Interfaces.Services.Security;
using BCryptNet = BCrypt.Net.BCrypt;
namespace SL.Infrastructre.Services.Security;

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
