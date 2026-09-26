using MilanSetu.API.Models;

namespace MilanSetu.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
