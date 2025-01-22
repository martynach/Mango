using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services;

public interface IJwtTokenGeneratorService
{
    public string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}