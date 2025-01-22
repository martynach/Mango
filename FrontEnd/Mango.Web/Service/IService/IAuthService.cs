using Mango.Web.Models;

namespace Mango.Web.Service.IService;

public interface IAuthService
{
    public Task<ResponseDto?> LoginAsync(LoginUserDto loginDto);
    public Task<ResponseDto?> RegisterAsync(RegisterUserDto registerDto);
    public Task<ResponseDto?> AssignRoleAsync(RegisterUserDto registerDto);
}